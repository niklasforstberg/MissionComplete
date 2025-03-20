namespace MissionComplete.Models.DTOs.OffSeason;

public class OffSeasonDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TeamId { get; set; }
    public DateTime CreatedAt { get; set; }
}