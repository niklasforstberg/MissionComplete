namespace MissionComplete.Models;

public enum GoalType
{
    Team,
    Individual
}

public enum GoalStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}

public enum GoalRecurrence
{
    None,
    Daily,
    Weekly,
    Monthly
}

public class Goal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GoalType Type { get; set; }
    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Recurrence properties
    public GoalRecurrence Recurrence { get; set; } = GoalRecurrence.None;
    public int TargetPerPeriod { get; set; } = 0;

    // Team relationship
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    // Optional user relationship (for individual goals)
    public int? UserId { get; set; }
    public User? User { get; set; }

    // Optional challenge relationship (if goal is tied to specific challenge types)
    public int? ChallengeId { get; set; }
    public Challenge? Challenge { get; set; }

    // Who created the goal
    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    // Navigation property for tracking progress
    public ICollection<GoalProgress> ProgressEntries { get; set; } = new List<GoalProgress>();
}

// Add this class to track progress for recurring goals
public class GoalProgress
{
    public int Id { get; set; }
    public int GoalId { get; set; }
    public Goal Goal { get; set; } = null!;

    // The period this progress entry is for (e.g., week number, month number)
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    // Progress tracking
    public int CurrentCount { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;

    // If this is for an individual user goal
    public int? UserId { get; set; }
    public User? User { get; set; }
}
