namespace MissionComplete.Models;

public enum UserRole
{
    Player,
    Coach,
    Admin
}

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; } = string.Empty;
    public bool? Invited { get; set; } = false;
    public int? InvitedById { get; set; }
    public User? InvitedBy { get; set; }
    public UserRole? Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Token { get; set; }
    public DateTime? TokenExpires { get; set; }

    public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
}