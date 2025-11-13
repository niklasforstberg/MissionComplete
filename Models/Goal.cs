namespace MissionComplete.Models;

public enum GoalRecurrence
{
    None,
    Daily,
    Weekly,
    Monthly,
    Season
}

public abstract class Goal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GoalRecurrence Recurrence { get; set; } = GoalRecurrence.None;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

}

public class TeamGoal : Goal
{
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
}

public class UserGoal : Goal
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}





