namespace MissionComplete.Models.DTOs.Team;

public class TeamCoachDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}