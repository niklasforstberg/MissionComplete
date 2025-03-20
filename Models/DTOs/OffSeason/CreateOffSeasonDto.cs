namespace MissionComplete.Models.DTOs.OffSeason;

public class CreateOffSeasonDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TeamId { get; set; }
}