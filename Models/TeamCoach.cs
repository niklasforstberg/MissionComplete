namespace MissionComplete.Models;

public class TeamCoach
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int CoachId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Team Team { get; set; } = null!;
    public User Coach { get; set; } = null!;
}