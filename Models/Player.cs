namespace MissionComplete.Models;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public ICollection<ChallengeCompletion> CompletedChallenges { get; set; } = new List<ChallengeCompletion>();
} 