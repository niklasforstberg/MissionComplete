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
        // Create challenge
        app.MapPost("/api/teams/{teamId}/challenges", async (int teamId, CreateChallengeDto request, HttpContext context, ApplicationDbContext db) =>
        {
            var team = await db.Teams.FindAsync(teamId);
            if (team == null)
                return Results.NotFound("Team not found");

            var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var challenge = new Challenge
            {
                Name = request.Name,
                Description = request.Description,
                Type = request.Type,
                Frequency = request.Frequency,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TeamId = teamId,
                CreatedById = userId
            };

            db.Challenges.Add(challenge);
            await db.SaveChangesAsync();

            return Results.Created($"/api/teams/{teamId}/challenges/{challenge.Id}", challenge);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Get team challenges
        app.MapGet("/api/teams/{teamId}/challenges", async (int teamId, ApplicationDbContext db) =>
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
                    CompletionCount = c.Completions.Count,
                    CreatedById = c.CreatedById
                })
                .ToListAsync();

            return Results.Ok(challenges);
        })
        .RequireAuthorization();

        // Get single challenge
        app.MapGet("/api/challenges/{id}", async (int id, ApplicationDbContext db) =>
        {
            var challenge = await db.Challenges
                .Include(c => c.Completions)
                .ThenInclude(cc => cc.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (challenge == null)
                return Results.NotFound();

            var response = new ChallengeDetailDto
            {
                Id = challenge.Id,
                Name = challenge.Name,
                Description = challenge.Description,
                Type = challenge.Type,
                Frequency = challenge.Frequency,
                StartDate = challenge.StartDate,
                EndDate = challenge.EndDate,
                Completions = challenge.Completions.Select(cc => new ChallengeCompletionDto
                {
                    Id = cc.Id,
                    UserId = cc.UserId,
                    UserEmail = cc.User.Email,
                    CompletedAt = cc.CompletedAt,
                    Notes = cc.Notes
                }).ToList()
            };

            return Results.Ok(response);
        })
        .RequireAuthorization();

        // Update challenge
        app.MapPut("/api/challenges/{id}", async (int id, UpdateChallengeDto request, ApplicationDbContext db) =>
        {
            var challenge = await db.Challenges.FindAsync(id);
            if (challenge == null)
                return Results.NotFound();

            challenge.Name = request.Name;
            challenge.Description = request.Description;
            challenge.Type = request.Type;
            challenge.Frequency = request.Frequency;
            challenge.StartDate = request.StartDate;
            challenge.EndDate = request.EndDate;

            await db.SaveChangesAsync();
            return Results.Ok(challenge);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Delete challenge
        app.MapDelete("/api/challenges/{id}", async (int id, ApplicationDbContext db) =>
        {
            var challenge = await db.Challenges.FindAsync(id);
            if (challenge == null)
                return Results.NotFound();

            db.Challenges.Remove(challenge);
            await db.SaveChangesAsync();

            return Results.Ok();
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));

        // Log challenge completion
        app.MapPost("/api/challenges/{id}/complete", async (int id, LogCompletionDto request, HttpContext context, ApplicationDbContext db) =>
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
                    CompletionCount = c.Completions.Count
                })
                .ToListAsync();

            return Results.Ok(challenges);
        })
        .RequireAuthorization(policy => policy.RequireRole("Coach", "Admin"));
    }
}