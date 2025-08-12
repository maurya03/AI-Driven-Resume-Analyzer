using System;
namespace ResumeAI.Models;
public class Candidate
{
    public int Id { get; set; }
    public string CandidateName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public decimal ExperienceYears { get; set; }
    public string? ResumeFilePath { get; set; }
    public string? RawText { get; set; }
    public string? AISummary { get; set; }
    public string? AISkills { get; set; }
    public double? AIScore { get; set; }
}
