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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WebIntelligenceContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<UserReminder> UserReminders { get; set; } = null!;
}