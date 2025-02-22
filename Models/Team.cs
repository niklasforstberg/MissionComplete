namespace MissionComplete.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Player> Players { get; set; } = new List<Player>();
    public ICollection<Challenge> Challenges { get; set; } = new List<Challenge>();
} 