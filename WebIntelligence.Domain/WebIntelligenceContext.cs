using Microsoft.EntityFrameworkCore;
using WebIntelligence.Domain.Model;

namespace WebIntelligence.Domain;

public class WebIntelligenceContext : DbContext
{
    public WebIntelligenceContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebIntelligenceContext).Assembly);
    }

    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<UserReminder> UserReminders { get; set; } = null!;
    public virtual DbSet<Poll> Polls { get; set; } = null!;
    public virtual DbSet<PollOption> PollOptions { get; set; } = null!;
    public virtual DbSet<UserVote> UserVotes { get; set; } = null!;
}