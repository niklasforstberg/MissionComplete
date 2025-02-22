namespace MissionComplete.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        group.MapGet("/me", GetCurrentPlayer)
            .WithName("GetCurrentPlayer")
            .WithOpenApi();
    }

    private static IResult GetCurrentPlayer()
    {
        return Results.Ok(new { Name = "TestPlayer" });
    }
} 