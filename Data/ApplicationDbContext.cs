using Microsoft.EntityFrameworkCore;
using MissionComplete.Models;
namespace MissionComplete.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add your DbSet properties here
    public DbSet<Team> Teams { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TeamUser> TeamUsers { get; set; }
    public DbSet<ChallengeCompletion> ChallengeCompletions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure many-to-many relationship
        modelBuilder.Entity<TeamUser>()
            .HasKey(tu => new { tu.TeamId, tu.UserId });

        modelBuilder.Entity<TeamUser>()
            .HasOne(tu => tu.Team)
            .WithMany(t => t.TeamUsers)
            .HasForeignKey(tu => tu.TeamId);

        modelBuilder.Entity<TeamUser>()
            .HasOne(tu => tu.User)
            .WithMany(u => u.TeamUsers)
            .HasForeignKey(tu => tu.UserId);
    }
} 