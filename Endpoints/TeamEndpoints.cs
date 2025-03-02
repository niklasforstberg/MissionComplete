using Microsoft.EntityFrameworkCore;
using MissionComplete.Data;
using MissionComplete.Models;
using MissionComplete.Models.DTOs;
using System.Security.Claims;

namespace MissionComplete.Endpoints;

public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/teams")
            .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Team management endpoints
        var managementGroup = group.MapGroup("")
            .WithTags("Team Management");
        MapTeamManagementEndpoints(managementGroup);

        // Team member management endpoints
        var memberGroup = group.MapGroup("")
            .WithTags("Team Members");
        MapTeamMemberEndpoints(memberGroup);
    }

    private static void MapTeamManagementEndpoints(IEndpointRouteBuilder group)
    {
        // Create team
        group.MapPost("/", async (CreateTeamRequest request, HttpContext context, ApplicationDbContext db) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await db.Users.FindAsync(userId);

            var team = new Team
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            var teamUser = new TeamUser
            {
                Team = team,
                UserId = userId,
                Role = TeamUser.TeamRole.Coach,
                JoinedAt = DateTime.UtcNow
            };

            db.Teams.Add(team);
            await db.SaveChangesAsync();

            var response = new TeamResponse(
                team.Id,
                team.Name,
                team.Description,
                team.CreatedAt,
                new[] 
                {
                    new TeamMemberResponse(
                        userId,
                        user!.Email,
                        TeamUser.TeamRole.Coach.ToString(),
                        teamUser.JoinedAt
                    )
                }
            );

            return Results.Created($"/api/teams/{team.Id}", response);
        });

        // Get all teams
        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var teams = await db.Teams
                .Select(t => new TeamListResponse(
                    t.Id,
                    t.Name,
                    t.Description,
                    t.TeamUsers.Count(tu => tu.Role == TeamUser.TeamRole.Player)
                ))
                .ToListAsync();

            return Results.Ok(teams);
        });

        // Get single team
        group.MapGet("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var team = await db.Teams
                .Include(t => t.TeamUsers)
                .ThenInclude(tu => tu.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return Results.NotFound();

            var response = new TeamResponse(
                team.Id,
                team.Name,
                team.Description,
                team.CreatedAt,
                team.TeamUsers.Select(tu => new TeamMemberResponse(
                    tu.User.Id,
                    tu.User.Email,
                    tu.Role.ToString(),
                    tu.JoinedAt
                ))
            );

            return Results.Ok(response);
        });

        // Update team
        group.MapPut("/{id}", async (int id, UpdateTeamRequest request, ApplicationDbContext db) =>
        {
            var team = await db.Teams.FindAsync(id);
            if (team == null)
                return Results.NotFound();

            team.Name = request.Name;
            team.Description = request.Description;
            await db.SaveChangesAsync();

            return Results.Ok(team);
        });

        // Delete team
        group.MapDelete("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var team = await db.Teams.FindAsync(id);
            if (team == null)
                return Results.NotFound();

            db.Teams.Remove(team);
            await db.SaveChangesAsync();

            return Results.Ok();
        });
    }

    private static void MapTeamMemberEndpoints(IEndpointRouteBuilder group)
    {
        // Add team member
        group.MapPost("/{id}/members", async (int id, AddTeamMemberRequest request, HttpContext context, ApplicationDbContext db) =>
        {
            var coachId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var team = await db.Teams.FindAsync(id);
            if (team == null)
                return Results.NotFound("Team not found");

            var user = await db.Users.FindAsync(request.UserId);
            if (user == null)
                return Results.NotFound("User not found");

            // Set invitation information
            user.Invited = true;
            user.InvitedById = coachId;

            var teamUser = new TeamUser
            {
                TeamId = id,
                UserId = request.UserId,
                Role = request.Role
            };

            team.TeamUsers.Add(teamUser);
            await db.SaveChangesAsync();

            return Results.Ok();
        });

        // Remove team member
        group.MapDelete("/{id}/members/{userId}", async (int id, int userId, ApplicationDbContext db) =>
        {
            var teamUser = await db.TeamUsers
                .FirstOrDefaultAsync(tu => tu.TeamId == id && tu.UserId == userId);

            if (teamUser == null)
                return Results.NotFound("User not found in team");

            db.TeamUsers.Remove(teamUser);
            await db.SaveChangesAsync();

            return Results.Ok();
        });
    }
}

public record CreateTeamRequest(string Name, string? Description);
public record UpdateTeamRequest(string Name, string? Description);
public record AddTeamMemberRequest(int UserId, TeamUser.TeamRole Role); 