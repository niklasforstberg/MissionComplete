using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MissionComplete.Data;
using MissionComplete.Models;
using MissionComplete.Models.DTOs.Goal;
using System.Security.Claims;

namespace MissionComplete.Endpoints;

public static class GoalEndpoints
{
    public static void MapGoalEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/goals").RequireAuthorization();

        // User Goals CRUD
        group.MapPost("/user", CreateUserGoal);
        group.MapGet("/user", GetUserGoals);
        group.MapGet("/user/{id}", GetUserGoalById);
        group.MapPut("/user/{id}", UpdateUserGoal);
        group.MapDelete("/user/{id}", DeleteUserGoal);

        // Team Goals CRUD
        group.MapPost("/team", CreateTeamGoal);
        group.MapGet("/team/{teamId}", GetTeamGoals);
        group.MapGet("/team/goal/{id}", GetTeamGoalById);
        group.MapPut("/team/goal/{id}", UpdateTeamGoal);
        group.MapDelete("/team/goal/{id}", DeleteTeamGoal);
    }

    // User Goals handlers
    private static async Task<IResult> CreateUserGoal(
        CreateUserGoalDto dto,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var goal = new UserGoal
        {
            Title = dto.Title,
            Description = dto.Description,
            Recurrence = dto.Recurrence,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatedById = userId,
            UserId = userId
        };

        db.UserGoals.Add(goal);
        await db.SaveChangesAsync();

        return Results.Created($"/api/goals/user/{goal.Id}", MapToUserGoalDto(goal));
    }

    private static async Task<IResult> GetUserGoals(
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var goals = await db.UserGoals
            .Include(g => g.CreatedBy)
            .Where(g => g.UserId == userId)
            .ToListAsync();

        return Results.Ok(goals.Select(MapToUserGoalDto));
    }

    private static async Task<IResult> GetUserGoalById(
        int id,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var goal = await db.UserGoals
            .Include(g => g.CreatedBy)
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal == null)
            return Results.NotFound();

        return Results.Ok(MapToUserGoalDto(goal));
    }

    private static async Task<IResult> UpdateUserGoal(
        int id,
        UpdateGoalDto dto,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var goal = await db.UserGoals
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal == null)
            return Results.NotFound();

        // Update only provided properties
        if (dto.Title != null) goal.Title = dto.Title;
        if (dto.Description != null) goal.Description = dto.Description;
        if (dto.Recurrence.HasValue) goal.Recurrence = dto.Recurrence.Value;
        if (dto.StartDate.HasValue) goal.StartDate = dto.StartDate.Value;
        goal.EndDate = dto.EndDate; // Can be set to null

        await db.SaveChangesAsync();
        return Results.Ok(MapToUserGoalDto(goal));
    }

    private static async Task<IResult> DeleteUserGoal(
        int id,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var goal = await db.UserGoals
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);

        if (goal == null)
            return Results.NotFound();

        db.UserGoals.Remove(goal);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    // Team Goals handlers
    private static async Task<IResult> CreateTeamGoal(
        CreateTeamGoalDto dto,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Check if user is a member of the team
        var isMember = await db.TeamUsers
            .AnyAsync(tu => tu.TeamId == dto.TeamId && tu.UserId == userId);

        if (!isMember)
            return Results.Forbid();

        var goal = new TeamGoal
        {
            Title = dto.Title,
            Description = dto.Description,
            Recurrence = dto.Recurrence,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatedById = userId,
            TeamId = dto.TeamId
        };

        db.TeamGoals.Add(goal);
        await db.SaveChangesAsync();

        return Results.Created($"/api/goals/team/goal/{goal.Id}", MapToTeamGoalDto(goal));
    }

    private static async Task<IResult> GetTeamGoals(
        int teamId,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Check if user is a member of the team
        var isMember = await db.TeamUsers
            .AnyAsync(tu => tu.TeamId == teamId && tu.UserId == userId);

        if (!isMember)
            return Results.Forbid();

        var goals = await db.TeamGoals
            .Include(g => g.CreatedBy)
            .Include(g => g.Team)
            .Where(g => g.TeamId == teamId)
            .ToListAsync();

        return Results.Ok(goals.Select(MapToTeamGoalDto));
    }

    private static async Task<IResult> GetTeamGoalById(
        int id,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var goal = await db.TeamGoals
            .Include(g => g.CreatedBy)
            .Include(g => g.Team)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (goal == null)
            return Results.NotFound();

        // Check if user is a member of the team
        var isMember = await db.TeamUsers
            .AnyAsync(tu => tu.TeamId == goal.TeamId && tu.UserId == userId);

        if (!isMember)
            return Results.Forbid();

        return Results.Ok(MapToTeamGoalDto(goal));
    }

    private static async Task<IResult> UpdateTeamGoal(
        int id,
        UpdateGoalDto dto,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var goal = await db.TeamGoals
            .Include(g => g.Team)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (goal == null)
            return Results.NotFound();

        // Check if user is a member of the team
        var isMember = await db.TeamUsers
            .AnyAsync(tu => tu.TeamId == goal.TeamId && tu.UserId == userId);

        if (!isMember)
            return Results.Forbid();

        // Update only provided properties
        if (dto.Title != null) goal.Title = dto.Title;
        if (dto.Description != null) goal.Description = dto.Description;
        if (dto.Recurrence.HasValue) goal.Recurrence = dto.Recurrence.Value;
        if (dto.StartDate.HasValue) goal.StartDate = dto.StartDate.Value;
        goal.EndDate = dto.EndDate; // Can be set to null

        await db.SaveChangesAsync();
        return Results.Ok(MapToTeamGoalDto(goal));
    }

    private static async Task<IResult> DeleteTeamGoal(
        int id,
        HttpContext context,
        ApplicationDbContext db)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var goal = await db.TeamGoals
            .FirstOrDefaultAsync(g => g.Id == id);

        if (goal == null)
            return Results.NotFound();

        // Check if user is a member of the team
        var isMember = await db.TeamUsers
            .AnyAsync(tu => tu.TeamId == goal.TeamId && tu.UserId == userId);

        if (!isMember)
            return Results.Forbid();

        db.TeamGoals.Remove(goal);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    // Helper methods for mapping entities to DTOs
    private static UserGoalDto MapToUserGoalDto(UserGoal goal)
    {
        return new UserGoalDto
        {
            Id = goal.Id,
            Title = goal.Title,
            Description = goal.Description,
            Recurrence = goal.Recurrence.ToString(),
            CreatedAt = goal.CreatedAt,
            CreatedById = goal.CreatedById,
            CreatedByEmail = goal.CreatedBy?.Email ?? string.Empty,
            StartDate = goal.StartDate,
            EndDate = goal.EndDate,
            UserId = goal.UserId
        };
    }

    private static TeamGoalDto MapToTeamGoalDto(TeamGoal goal)
    {
        return new TeamGoalDto
        {
            Id = goal.Id,
            Title = goal.Title,
            Description = goal.Description,
            Recurrence = goal.Recurrence.ToString(),
            CreatedAt = goal.CreatedAt,
            CreatedById = goal.CreatedById,
            CreatedByEmail = goal.CreatedBy?.Email ?? string.Empty,
            StartDate = goal.StartDate,
            EndDate = goal.EndDate,
            TeamId = goal.TeamId,
            TeamName = goal.Team?.Name ?? string.Empty
        };
    }
}