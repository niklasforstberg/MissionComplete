using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MissionComplete.Data;
using MissionComplete.Models;
using MissionComplete.Models.DTOs.OffSeason;

namespace MissionComplete.Endpoints;

public static class OffSeasonEndpoints
{
    public static void MapOffSeasonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/offseasons")
            .RequireAuthorization();

        // Get all off seasons for a team
        group.MapGet("team/{teamId}", async (int teamId, ApplicationDbContext db) =>
        {
            var offSeasons = await db.OffSeasons
                .Where(o => o.TeamId == teamId)
                .OrderByDescending(o => o.StartDate)
                .Select(o => new OffSeasonDto
                {
                    Id = o.Id,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    TeamId = o.TeamId,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return Results.Ok(offSeasons);
        });

        // Get a specific off season
        group.MapGet("{id}", async (int id, ApplicationDbContext db) =>
        {
            var offSeason = await db.OffSeasons.FindAsync(id);

            if (offSeason == null)
                return Results.NotFound();

            return Results.Ok(new OffSeasonDto
            {
                Id = offSeason.Id,
                StartDate = offSeason.StartDate,
                EndDate = offSeason.EndDate,
                TeamId = offSeason.TeamId,
                CreatedAt = offSeason.CreatedAt
            });
        });

        // Create a new off season
        group.MapPost("", async (CreateOffSeasonDto request, ApplicationDbContext db) =>
        {
            // Validate team exists
            var team = await db.Teams.FindAsync(request.TeamId);
            if (team == null)
                return Results.NotFound("Team not found");

            // Validate dates
            if (request.EndDate <= request.StartDate)
                return Results.BadRequest("End date must be after start date");

            var offSeason = new OffSeason
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TeamId = request.TeamId
            };

            db.OffSeasons.Add(offSeason);
            await db.SaveChangesAsync();

            return Results.Created($"/api/offseasons/{offSeason.Id}", new OffSeasonDto
            {
                Id = offSeason.Id,
                StartDate = offSeason.StartDate,
                EndDate = offSeason.EndDate,
                TeamId = offSeason.TeamId,
                CreatedAt = offSeason.CreatedAt
            });
        });

        // Update an off season
        group.MapPut("{id}", async (int id, UpdateOffSeasonDto request, ApplicationDbContext db) =>
        {
            var offSeason = await db.OffSeasons.FindAsync(id);

            if (offSeason == null)
                return Results.NotFound();

            // Validate dates
            if (request.EndDate <= request.StartDate)
                return Results.BadRequest("End date must be after start date");

            offSeason.StartDate = request.StartDate;
            offSeason.EndDate = request.EndDate;

            await db.SaveChangesAsync();

            return Results.Ok(new OffSeasonDto
            {
                Id = offSeason.Id,
                StartDate = offSeason.StartDate,
                EndDate = offSeason.EndDate,
                TeamId = offSeason.TeamId,
                CreatedAt = offSeason.CreatedAt
            });
        });

        // Delete an off season
        group.MapDelete("{id}", async (int id, ApplicationDbContext db) =>
        {
            var offSeason = await db.OffSeasons.FindAsync(id);

            if (offSeason == null)
                return Results.NotFound();

            db.OffSeasons.Remove(offSeason);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}