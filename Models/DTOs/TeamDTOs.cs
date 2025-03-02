namespace MissionComplete.Models.DTOs;

public record CreateTeamRequest(string Name, string? Description);
public record UpdateTeamRequest(string Name, string? Description);
public record AddTeamMemberRequest(int UserId, TeamUser.TeamRole Role);

public record TeamResponse(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    IEnumerable<TeamMemberResponse> Members
);

public record TeamListResponse(
    int Id,
    string Name,
    string? Description,
    int PlayerCount
);

public record TeamMemberResponse(
    int UserId,
    string Email,
    string Role,
    DateTime JoinedAt
); 