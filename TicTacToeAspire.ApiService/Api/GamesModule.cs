using GameEngineService.Application;
namespace GameEngineService.Api;

public record MoveRequest(int Position, string Player);
public static class GamesModule
{
    public static void MapGamesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/games").WithTags("Games");

        group.MapPost("/{gameId}", (string gameId, IGameLogic gameLogic) => {
            var game = gameLogic.CreateGame(gameId);
            return Results.Created($"/games/{gameId}", game);
        });

        group.MapGet("/{gameId}", (string gameId, IGameLogic gameLogic) => {
            var game = gameLogic.GetGame(gameId);
            return game is null ? Results.NotFound() : Results.Ok(game);
        });

        group.MapPost("/{gameId}/move", (string gameId, MoveRequest move, IGameLogic gameLogic) => {
            try
            {
                var game = gameLogic.MakeMove(gameId, move.Position, move.Player);
                return Results.Ok(game);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        });
    }
}