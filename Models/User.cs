namespace MissionComplete.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();

    public enum UserRole
    {
        Player,
        Coach,
        Admin
    }
} 