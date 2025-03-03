namespace MissionComplete.Models.DTOs.Auth;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public TeamUser.TeamRole Role { get; set; }
} 