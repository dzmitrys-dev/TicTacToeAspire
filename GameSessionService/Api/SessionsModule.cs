using GameSessionService.Application;
namespace GameSessionService.Api;

public static class SessionsModule
{
    public static void MapSessionsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/sessions").WithTags("Sessions");

        group.MapPost("/", async (ISessionManager sessionManager) => {
            var session = await sessionManager.CreateSessionAsync();
            return Results.Created($"/sessions/{session.SessionId}", session);
        });

        group.MapGet("/{sessionId}", (string sessionId, ISessionManager sessionManager) => {
            var session = sessionManager.GetSession(sessionId);
            return session is null ? Results.NotFound() : Results.Ok(session);
        });

        group.MapPost("/{sessionId}/simulate", (string sessionId, ISessionManager sessionManager) => {
            _ = sessionManager.StartSimulationAsync(sessionId);
            return Results.Accepted();
        });
    }
}