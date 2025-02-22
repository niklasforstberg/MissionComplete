namespace MissionComplete.Models;

public enum ChallengeType
{
    Cardio,
    Strength,
    SkillBased,
    Other
}

public enum ChallengeFrequency
{
    Daily,
    Weekly,
    Custom
}

public class Challenge
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ChallengeType Type { get; set; }
    public ChallengeFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public ICollection<ChallengeCompletion> Completions { get; set; } = new List<ChallengeCompletion>();
} 