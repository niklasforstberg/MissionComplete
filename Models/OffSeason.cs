using System;

namespace MissionComplete.Models;

public class OffSeason
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedById { get; set; }

    // Foreign key
    public int TeamId { get; set; }

    // Navigation property
    public Team Team { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;

}