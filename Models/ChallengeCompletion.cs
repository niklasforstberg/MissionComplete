namespace MissionComplete.Models;

public class ChallengeCompletion
{
    public int Id { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    
    // Navigation properties
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public int ChallengeId { get; set; }
    public Challenge Challenge { get; set; } = null!;
} 