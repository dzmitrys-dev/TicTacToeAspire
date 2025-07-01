using GameEngineService.Domain;
namespace GameEngineService.Application;

public interface IGameRepository { Task<Game?> FindByIdAsync(string gameId); Task SaveAsync(Game game); }
