using System.Collections.Concurrent;

namespace GameEngineService.Infrastructure;

public interface IGameLockProvider
{
    object GetLockForGame(string gameId);
}

public class GameLockProvider : IGameLockProvider
{
    private readonly ConcurrentDictionary<string, object> _locks = new();

    public object GetLockForGame(string gameId)
    {
        // GetOrAdd ensures that for any given gameId, we always get back the same lock object.
        return _locks.GetOrAdd(gameId, _ => new object());
    }
}