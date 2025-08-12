using UglyToad.PdfPig;
using System.Text.Json;
using ResumeAI.Data;
using ResumeAI.Models;
namespace ResumeAI.Services;
public class ResumeService
{
    private readonly AIService _ai;
    private readonly AppDbContext _db;
    private readonly SearchService _search;
    public ResumeService(AIService ai, AppDbContext db, SearchService search) { _ai = ai; _db = db; _search = search; }

    public async Task ParseAndAnalyzeAsync(int candidateId, string filePath)
    {
        // Extract text (PDF) - for simplicity only PDF supported in this template
        string text = string.Empty;
        try {
            using var doc = PdfDocument.Open(filePath);
            foreach(var p in doc.GetPages()) text += p.Text + "\n";
        } catch { text = File.ReadAllText(filePath); }

        // call AI for structured JSON output
        var prompt = $"Extract top 5 skills as comma-separated, a 2-sentence professional summary, and a suitability score 0-100 for a generic software engineer from the resume text:\n\n{text}";
        var completion = await _ai.GetCompletionAsync(prompt);

        // Naive parsing: try find skills line and score using regex - production: request JSON from model
        var skills = "";
        var score = 0.0;
        try {
            var sMatch = System.Text.RegularExpressions.Regex.Match(completion, "(?i)skills?:\\s*(.*)");
            if (sMatch.Success) skills = sMatch.Groups[1].Value.Split('\n')[0].Trim();
            var sc = System.Text.RegularExpressions.Regex.Match(completion, "\\b(\\d{1,3})\\b");
            if (sc.Success) score = double.Parse(sc.Groups[1].Value);
        } catch {}
        var summary = completion.Length>500?completion.Substring(0,500):completion;

        var cand = await _db.Candidates.FindAsync(candidateId);
        if (cand != null) {
            cand.RawText = text; cand.AISkills = skills; cand.AISummary = summary; cand.AIScore = score;
            await _db.SaveChangesAsync();

            // embeddings
            var embJson = await _ai.GetEmbeddingAsync(text);
            await _search.UpsertEmbeddingAsync(candidateId, embJson);
        }
    }
}
