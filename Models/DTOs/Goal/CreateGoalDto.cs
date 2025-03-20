namespace MissionComplete.Models.DTOs.Goal;

public class CreateGoalDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GoalRecurrence Recurrence { get; set; } = GoalRecurrence.None;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CreateTeamGoalDto : CreateGoalDto
{
    public int TeamId { get; set; }
}

public class CreateUserGoalDto : CreateGoalDto
{
    // UserId will be derived from the authenticated user
}