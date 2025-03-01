namespace MissionComplete.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
} 