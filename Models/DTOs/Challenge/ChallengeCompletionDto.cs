namespace MissionComplete.Models.DTOs.Challenge;

public class ChallengeCompletionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public string? Notes { get; set; }
} 