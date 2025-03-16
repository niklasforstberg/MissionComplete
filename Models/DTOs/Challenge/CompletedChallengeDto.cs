namespace MissionComplete.Models.DTOs.Challenge;

public class CompletedChallengeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int CompletionId { get; set; }
    public DateTime CompletedAt { get; set; }
    public string? Notes { get; set; }
}