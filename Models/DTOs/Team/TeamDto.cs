namespace MissionComplete.Models.DTOs.Team;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<TeamCoachDto> Coaches { get; set; } = new List<TeamCoachDto>();
    public ICollection<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
}