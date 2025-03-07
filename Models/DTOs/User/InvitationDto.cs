using System;
public class InvitationDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int InviterId { get; set; }
    public required string InviteeEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Token { get; set; }
    public string? TeamName { get; set; }
    public string? InviterName { get; set; }
}
