using MissionComplete.Models;

public class TeamUser
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}