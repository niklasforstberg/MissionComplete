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
    Monthly,
    OneTime,
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
    public int TeamId { get; set; }
    public int CreatedById { get; set; }
    // Navigation properties
    public Team Team { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
}