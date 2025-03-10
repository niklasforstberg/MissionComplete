using Microsoft.EntityFrameworkCore;
using MissionComplete.Data;
using MissionComplete.Models;
using MissionComplete.Integrations;
using MissionComplete.Models.DTOs.Team;
using System.Security.Claims;

namespace MissionComplete.Endpoints;

public static class TeamEndpoints
{
    public static void MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        // Create team
        app.MapPost("/api/teams", async (CreateTeamDto request, HttpContext context, ApplicationDbContext db) =>
        {
            var coachId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var coach = await db.Users.FindAsync(coachId);

            var team = new Team
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            var teamCoach = new TeamCoach
            {
                Team = team,
                CoachId = coachId,
                JoinedAt = DateTime.UtcNow
            };

            db.Teams.Add(team);
            await db.SaveChangesAsync();

            var response = new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                CreatedAt = team.CreatedAt,
                Coaches = new List<TeamCoachDto>
                {
                    new TeamCoachDto
                    {
                        UserId = coachId,
                        Email = coach!.Email,
                        JoinedAt = teamCoach.JoinedAt
                    }
                },
                Members = new List<TeamMemberDto>()
            };

            return Results.Created($"/api/teams/{team.Id}", response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Get all teams
        app.MapGet("/api/teams", async (ApplicationDbContext db) =>
        {
            var teams = await db.Teams
                .Select(t => new TeamListDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description
                })
                .ToListAsync();

            return Results.Ok(teams);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Get single team
        app.MapGet("/api/teams/{id}", async (int id, ApplicationDbContext db) =>
        {
            var team = await db.Teams
                .Include(t => t.TeamUsers)
                .ThenInclude(tu => tu.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return Results.NotFound();

            var response = new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                CreatedAt = team.CreatedAt,
                Members = team.TeamUsers.Select(tu => new TeamMemberDto
                {
                    UserId = tu.User.Id,
                    Email = tu.User.Email,
                    JoinedAt = tu.JoinedAt
                }).ToList()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Update team
        app.MapPut("/api/teams/{id}", async (int id, UpdateTeamDto request, ApplicationDbContext db) =>
        {
            var team = await db.Teams.FindAsync(id);
            if (team == null)
                return Results.NotFound();

            team.Name = request.Name;
            team.Description = request.Description;
            await db.SaveChangesAsync();

            var response = new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                CreatedAt = team.CreatedAt,
                Members = new List<TeamMemberDto>()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Delete team
        app.MapDelete("/api/teams/{id}", async (int id, ApplicationDbContext db) =>
        {
            var team = await db.Teams.FindAsync(id);
            if (team == null)
                return Results.NotFound();

            db.Teams.Remove(team);
            await db.SaveChangesAsync();

            return Results.Ok();
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Add team member
        app.MapPost("/api/teams/{id}/members", async (int id, AddTeamMemberDto request, HttpContext context, ApplicationDbContext db, SmtpEmailSender emailSender) =>
        {
            var coachId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var team = await db.Teams.FindAsync(id);
            if (team == null)
                return Results.NotFound("Team not found");

            //If the user does not exist, create them
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                var token = GenerateSecureToken();
                user = new User
                {
                    Email = request.Email,
                    Invited = true,
                    InvitedById = coachId,
                    Role = User.UserRole.Player,
                    Token = token,
                    TokenExpires = DateTime.UtcNow.AddHours(48)
                };
                db.Users.Add(user);

                await emailSender.SendInvitationEmail(new InvitationDto
                {
                    InviteeEmail = request.Email,
                    Token = token,
                    TeamName = team.Name,
                    InviterName = user.Email
                });
            }
            else
            {
                user.Invited = true;
                user.InvitedById = coachId;
            }

            var teamUser = new TeamUser
            {
                TeamId = id,
                UserId = user.Id,
                JoinedAt = DateTime.UtcNow
            };

            team.TeamUsers.Add(teamUser);
            await db.SaveChangesAsync();

            var response = new TeamMemberDto
            {
                UserId = user.Id,
                Email = user.Email,
                Role = request.Role.ToString(),
                JoinedAt = teamUser.JoinedAt
            };

            return Results.Ok(response);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Remove team member
        app.MapDelete("/api/teams/{id}/members/{userId}", async (int id, int userId, ApplicationDbContext db) =>
        {
            var teamUser = await db.TeamUsers
                .FirstOrDefaultAsync(tu => tu.TeamId == id && tu.UserId == userId);

            if (teamUser == null)
                return Results.NotFound("User not found in team");

            db.TeamUsers.Remove(teamUser);
            await db.SaveChangesAsync();

            return Results.Ok();
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Replace("=", "");
    }
}

public record CreateTeamRequest(string Name, string? Description);
public record UpdateTeamRequest(string Name, string? Description);
public record AddTeamMemberDto(string Email, TeamUser.TeamRole Role);