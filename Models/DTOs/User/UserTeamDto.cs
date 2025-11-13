namespace MissionComplete.Models.DTOs.User;

public class UserTeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

