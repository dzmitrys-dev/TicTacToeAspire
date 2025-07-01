using GameSessionService.Domain;
namespace GameSessionService.Application;

public interface IGameEngineClient { Task<GameState?> CreateGameAsync(string gameId); Task<GameState?> MakeMoveAsync(string gameId, int position, string player); }
