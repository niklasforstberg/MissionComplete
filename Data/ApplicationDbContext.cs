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
    public DbSet<Player> Players { get; set; }
    public DbSet<ChallengeCompletion> ChallengeCompletions { get; set; }
    public DbSet<User> Users { get; set; }
} 