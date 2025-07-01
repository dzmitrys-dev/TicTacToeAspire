using System.Collections.Concurrent;
using GameEngineService.Application;
using GameEngineService.Domain;
namespace GameEngineService.Infrastructure;

public class InMemoryGameRepository : IGameRepository
{
    private readonly ConcurrentDictionary<string, Game> _games = new();
    public Task<Game?> FindByIdAsync(string gameId) => Task.FromResult(_games.GetValueOrDefault(gameId));
    public Task SaveAsync(Game game)
    {
        _games[game.GameId] = game;
        return Task.CompletedTask;
    }
}