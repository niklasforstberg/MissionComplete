using Microsoft.EntityFrameworkCore;
using MissionComplete.Data;
using MissionComplete.Models.DTOs.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MissionComplete.Models.DTOs.Challenge;

namespace MissionComplete.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/by-email/{email}", async (string email, ApplicationDbContext db) =>
        {
            var user = await db.Users
                .Include(u => u.TeamUsers)
                    .ThenInclude(tu => tu.Team)
                .Include(u => u.InvitedBy)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Results.NotFound();

            var response = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Role = user.Role?.ToString() ?? "User",
                Invited = user.Invited,
                InvitedBy = user.InvitedBy == null ? null : new UserInviterDto
                {
                    Id = user.InvitedBy.Id,
                    Email = user.InvitedBy.Email ?? "",
                    Role = user.InvitedBy.Role?.ToString() ?? "User"
                },
                Teams = user.TeamUsers.Select(tu => new UserTeamDto
                {
                    Id = tu.Team.Id,
                    Name = tu.Team.Name,
                    JoinedAt = tu.JoinedAt
                }).ToList()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Get all teams for a user
        app.MapGet("/api/user/teams", async (ClaimsPrincipal user, ApplicationDbContext db) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Results.Unauthorized();

            var dbUser = await db.Users
                .Include(u => u.TeamUsers)
                    .ThenInclude(tu => tu.Team)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (dbUser == null)
                return Results.NotFound();

            var teams = dbUser.TeamUsers.Select(tu => new UserTeamDto
            {
                Id = tu.Team.Id,
                Name = tu.Team.Name,
                JoinedAt = tu.JoinedAt
            }).ToList();

            return Results.Ok(teams);
        })
        .RequireAuthorization();

        // Get user's completed challenges
        app.MapGet("/api/user/completed-challenges", async (ClaimsPrincipal user, ApplicationDbContext db) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Results.Unauthorized();

            var completions = await db.ChallengeCompletions
                .Where(cc => cc.UserId == int.Parse(userId))
                .Include(cc => cc.Challenge)
                    .ThenInclude(c => c.Team)
                .OrderByDescending(cc => cc.CompletedAt)
                .Select(cc => new CompletedChallengeDto
                {
                    Id = cc.Challenge.Id,
                    Name = cc.Challenge.Name,
                    Description = cc.Challenge.Description,
                    Type = cc.Challenge.Type.ToString(),
                    Frequency = cc.Challenge.Frequency.ToString(),
                    StartDate = cc.Challenge.StartDate,
                    EndDate = cc.Challenge.EndDate,
                    TeamId = cc.Challenge.TeamId,
                    TeamName = cc.Challenge.Team.Name,
                    CompletionId = cc.Id,
                    CompletedAt = cc.CompletedAt,
                    Notes = cc.Notes
                })
                .ToListAsync();

            return Results.Ok(completions);
        })
        .RequireAuthorization();
    }
}