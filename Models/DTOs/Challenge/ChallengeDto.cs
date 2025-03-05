namespace MissionComplete.Models.DTOs.Challenge;

public class ChallengeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ChallengeType Type { get; set; }
    public ChallengeFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CompletionCount { get; set; }
    public int CreatedById { get; set; }
}