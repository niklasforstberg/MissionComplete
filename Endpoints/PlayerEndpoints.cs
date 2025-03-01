namespace MissionComplete.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api");

        // The /me endpoint has been moved to AuthEndpoints
    }
} 