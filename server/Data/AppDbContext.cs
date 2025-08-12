using Microsoft.EntityFrameworkCore;
using ResumeAI.Models;
namespace ResumeAI.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Candidate> Candidates => Set<Candidate>();
}
