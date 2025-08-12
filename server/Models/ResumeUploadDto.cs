using Microsoft.AspNetCore.Http;
namespace ResumeAI.Models;
public class ResumeUploadDto { public string CandidateName { get; set; } = null!; public string Email { get; set; } = null!; public decimal ExperienceYears { get; set; } public IFormFile? ResumeFile { get; set; } }
