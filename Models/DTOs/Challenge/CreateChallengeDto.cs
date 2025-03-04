namespace MissionComplete.Models.DTOs.Challenge;

public class CreateChallengeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ChallengeType Type { get; set; }
    public ChallengeFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
} 