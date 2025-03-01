using Microsoft.EntityFrameworkCore;

namespace MissionComplete.Models;

public class ChallengeCompletion
{
    public int Id { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    
    // Navigation properties
    public int ChallengeId { get; set; }
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Challenge Challenge { get; set; } = null!;
    
    public int UserId { get; set; }
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public User User { get; set; } = null!;
} 