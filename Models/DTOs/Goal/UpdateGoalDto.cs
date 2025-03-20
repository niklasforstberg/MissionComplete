namespace MissionComplete.Models.DTOs.Goal;

public class UpdateGoalDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public GoalRecurrence? Recurrence { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}