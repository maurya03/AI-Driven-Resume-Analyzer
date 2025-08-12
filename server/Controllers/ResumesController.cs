using Microsoft.AspNetCore.Mvc;
using ResumeAI.Data;
using ResumeAI.Models;
using ResumeAI.Services;

namespace ResumeAI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ResumesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ResumeService _resumeService;
    private readonly SearchService _searchService;

    public ResumesController(AppDbContext db, ResumeService resumeService, SearchService searchService)
    {
        _db = db; _resumeService = resumeService; _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Candidates.OrderByDescending(c => c.AIScore ?? 0).ToListAsync();
        return Ok(list);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] ResumeUploadDto dto)
    {
        if (dto.ResumeFile == null) return BadRequest("File required");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads"); Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, Guid.NewGuid()+"_"+dto.ResumeFile.FileName);
        using (var fs = new FileStream(filePath, FileMode.Create)) await dto.ResumeFile.CopyToAsync(fs);

        var cand = new Candidate { CandidateName = dto.CandidateName, Email = dto.Email, ExperienceYears = dto.ExperienceYears, ResumeFilePath = filePath };
        _db.Candidates.Add(cand); await _db.SaveChangesAsync();

        // parse and AI analyze
        await _resumeService.ParseAndAnalyzeAsync(cand.Id, filePath);

        // return updated candidate
        var updated = await _db.Candidates.FindAsync(cand.Id);
        return Ok(updated);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SemanticSearch([FromBody] SearchRequestDto dto)
    {
        var results = await _searchService.SemanticSearchAsync(dto.Query, dto.TopK ?? 10);
        return Ok(results);
    }
}
