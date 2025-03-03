namespace MissionComplete.Models.DTOs.Team;

public class AddTeamMemberDto
{
    public string Email { get; set; } = string.Empty;
    public TeamUser.TeamRole Role { get; set; }
} 