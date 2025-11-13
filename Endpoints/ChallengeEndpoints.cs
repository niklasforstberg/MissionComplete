using Microsoft.EntityFrameworkCore;
using MissionComplete.Data;
using MissionComplete.Models;
using MissionComplete.Models.DTOs.Challenge;
using System.Security.Claims;


namespace MissionComplete.Endpoints;

public static class ChallengeEndpoints
{
    public static void MapChallengeEndpoints(this IEndpointRouteBuilder app)
    {
        // Create challenge - allow both coaches and players
        app.MapPost("/api/challenge", async (CreateChallengeDto request, HttpContext context, ApplicationDbContext db) =>
        {
            var team = await db.Teams.FindAsync(request.TeamId);
            if (team == null)
                return Results.NotFound("Team not found");

            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Check if user is a member of the team
            var isTeamMember = await db.TeamUsers.AnyAsync(tu =>
                tu.TeamId == request.TeamId && tu.UserId == userId);

            if (!isTeamMember && !context.User.IsInRole("Coach") && !context.User.IsInRole("Admin"))
                return Results.Forbid();

            var challenge = new Challenge
            {
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                Frequency = request.Frequency,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TeamId = request.TeamId,
                CreatedById = userId
            };

            db.Challenges.Add(challenge);
            await db.SaveChangesAsync();

            var responseDto = new ChallengeDto
            {
                Id = challenge.Id,
                Name = challenge.Name,
                Description = challenge.Description,
                Type = challenge.Type,
                Frequency = challenge.Frequency,
                StartDate = challenge.StartDate,
                EndDate = challenge.EndDate,
                CreatedById = challenge.CreatedById,
                TeamId = challenge.TeamId
            };

            return Results.Created($"/api/challenges/{challenge.Id}", responseDto);
        })
        .RequireAuthorization(); // Allow any authenticated user

        // Get team challenges
        app.MapGet("/api/team/{teamId}/challenges", async (int teamId, ApplicationDbContext db) =>
        {
            var challenges = await db.Challenges
                .Where(c => c.TeamId == teamId)
                .Select(c => new ChallengeDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Type = c.Type,
                    Frequency = c.Frequency,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    CreatedById = c.CreatedById,
                    TeamId = c.TeamId
                })
                .ToListAsync();

            return Results.Ok(challenges);
        })
        .RequireAuthorization();

        // Get single challenge
        app.MapGet("/api/challenge/{id}", async (int id, HttpContext context, ApplicationDbContext db) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var challenge = await db.Challenges
                .Include(c => c.Team)
                .ThenInclude(t => t.TeamUsers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (challenge == null)
                return Results.NotFound();

            // Check if user is member of the team
            if (!challenge.Team.TeamUsers.Any(tu => tu.UserId == userId))
                return Results.NotFound();

            var response = new ChallengeDto
            {
                Id = challenge.Id,
                Name = challenge.Name,
                Description = challenge.Description,
                Type = challenge.Type,
                Frequency = challenge.Frequency,
                StartDate = challenge.StartDate,
                EndDate = challenge.EndDate,
                CreatedById = challenge.CreatedById,
                TeamId = challenge.TeamId
            };

            return Results.Ok(response);
        })
        .RequireAuthorization();

        // Update challenge - only allow the creator to update
        app.MapPut("/api/challenge/{id}", async (int id, UpdateChallengeDto request, HttpContext context, ApplicationDbContext db) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var challenge = await db.Challenges.FindAsync(id);

            if (challenge == null)
                return Results.NotFound();

            if (challenge.CreatedById != userId)
                return Results.Forbid();

            challenge.Name = request.Name;
            challenge.Description = request.Description;
            challenge.Type = request.Type;
            challenge.Frequency = request.Frequency;
            challenge.StartDate = request.StartDate;
            challenge.EndDate = request.EndDate;
            challenge.TeamId = request.TeamId;

            await db.SaveChangesAsync();
            return Results.Ok(challenge);
        })
        .RequireAuthorization();

        // Delete challenge - only allow the creator to delete
        app.MapDelete("/api/challenge/{id}", async (int id, HttpContext context, ApplicationDbContext db) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var challenge = await db.Challenges.FindAsync(id);

            if (challenge == null)
                return Results.NotFound();

            if (challenge.CreatedById != userId)
                return Results.Forbid();

            db.Challenges.Remove(challenge);
            await db.SaveChangesAsync();

            return Results.Ok();
        })
        .RequireAuthorization();

        // Log challenge completion
        app.MapPost("/api/challenge/{id}/complete", async (int id, LogCompletionDto request, HttpContext context, ApplicationDbContext db) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var completion = new ChallengeCompletion
            {
                ChallengeId = id,
                UserId = userId,
                Notes = request.Notes,
                CompletedAt = DateTime.UtcNow
            };

            db.ChallengeCompletions.Add(completion);
            await db.SaveChangesAsync();

            return Results.Ok(completion);
        })
        .RequireAuthorization();

        // Get challenges created by user
        app.MapGet("/api/challenges/my", async (HttpContext context, ApplicationDbContext db) =>
        {
            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var challenges = await db.Challenges
                .Where(c => c.CreatedById == userId)
                .Select(c => new ChallengeDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Type = c.Type,
                    Frequency = c.Frequency,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    CreatedById = c.CreatedById,
                    TeamId = c.TeamId
                })
                .ToListAsync();

            return Results.Ok(challenges);
        })
        .RequireAuthorization();
    }
}