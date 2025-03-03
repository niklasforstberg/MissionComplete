namespace MissionComplete.Models.DTOs.Team;

public class TeamMemberDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
} 