using GameSessionService.Domain;
namespace GameSessionService.Application;

public interface ISessionManager { Task<Session> CreateSessionAsync(); Task StartSimulationAsync(string sessionId); Session? GetSession(string sessionId); }
