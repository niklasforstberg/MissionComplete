using Microsoft.EntityFrameworkCore;
using MissionComplete.Data;
using MissionComplete.Models;
using System.Security.Claims;

namespace MissionComplete.Endpoints;

public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/teams")
            .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        group.MapPost("/", async (CreateTeamRequest request, HttpContext context, ApplicationDbContext db) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var team = new Team
            {
                Name = request.Name,
                Description = request.Description,
                TeamUsers = new List<TeamUser>
                {
                    new() { UserId = userId, Role = TeamUser.TeamRole.Coach }
                }
            };

            db.Teams.Add(team);
            await db.SaveChangesAsync();

            return Results.Created($"/api/teams/{team.Id}", team);
        });

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var teams = await db.Teams
                .Include(t => t.TeamUsers)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Description,
                    PlayerCount = t.TeamUsers.Count(tu => tu.Role == TeamUser.TeamRole.Player)
                })
                .ToListAsync();

            return Results.Ok(teams);
        });

        group.MapGet("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var team = await db.Teams
                .Include(t => t.TeamUsers)
                .ThenInclude(tu => tu.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return Results.NotFound();

            return Results.Ok(new
            {
                team.Id,
                team.Name,
                team.Description,
                Players = team.TeamUsers
                    .Where(tu => tu.Role == TeamUser.TeamRole.Player)
                    .Select(tu => new
                    {
                        id = tu.User.Id,
                        email = tu.User.Email,
                        joinedAt = tu.JoinedAt
                    }),
                Coaches = team.TeamUsers
                    .Where(tu => tu.Role == TeamUser.TeamRole.Coach)
                    .Select(tu => new
                    {
                        id = tu.User.Id,
                        email = tu.User.Email,
                        joinedAt = tu.JoinedAt
                    })
            });
        });

        group.MapPost("/{id}/members", async (int id, AddTeamMemberRequest request, ApplicationDbContext db) =>
        {
            var team = await db.Teams.FindAsync(id);
            if (team == null)
                return Results.NotFound("Team not found");

            var user = await db.Users.FindAsync(request.UserId);
            if (user == null)
                return Results.NotFound("User not found");

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
}

public record CreateTeamRequest(string Name, string? Description);
public record UpdateTeamRequest(string Name, string? Description);
public record AddTeamMemberRequest(int UserId, TeamUser.TeamRole Role); 