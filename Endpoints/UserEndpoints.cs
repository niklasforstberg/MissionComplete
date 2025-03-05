using Microsoft.EntityFrameworkCore;
using MissionComplete.Data;
using MissionComplete.Models.DTOs.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
                    Role = tu.Role.ToString(),
                    JoinedAt = tu.JoinedAt
                }).ToList()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

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
                Role = tu.Role.ToString(),
                JoinedAt = tu.JoinedAt
            }).ToList();

            return Results.Ok(teams);
        })
        .RequireAuthorization();
    }
}