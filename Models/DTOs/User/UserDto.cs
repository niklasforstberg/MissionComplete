using MissionComplete.Models.DTOs.Team;

namespace MissionComplete.Models.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool? Invited { get; set; }
    public UserInviterDto? InvitedBy { get; set; }
    public ICollection<UserTeamDto> Teams { get; set; } = new List<UserTeamDto>();
}

