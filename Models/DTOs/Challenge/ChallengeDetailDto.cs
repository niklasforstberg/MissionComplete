namespace MissionComplete.Models.DTOs.Challenge;

public class ChallengeDetailDto : ChallengeDto
{
    public List<ChallengeCompletionDto> Completions { get; set; } = new();
} 