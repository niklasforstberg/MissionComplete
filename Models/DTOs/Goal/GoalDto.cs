namespace MissionComplete.Models.DTOs.Goal;

public class GoalDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Recurrence { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int CreatedById { get; set; }
    public string CreatedByEmail { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class TeamGoalDto : GoalDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
}

public class UserGoalDto : GoalDto
{
    public int UserId { get; set; }
}