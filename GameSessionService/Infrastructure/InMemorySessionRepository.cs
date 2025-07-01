using System.Collections.Concurrent;
using GameSessionService.Application;
using GameSessionService.Domain;
namespace GameSessionService.Infrastructure;

public class InMemorySessionRepository : ISessionRepository
{
    private readonly ConcurrentDictionary<string, Session> _sessions = new();
    public Task<Session?> FindByIdAsync(string sessionId) => Task.FromResult(_sessions.GetValueOrDefault(sessionId));
    public Task SaveAsync(Session session) { _sessions[session.SessionId] = session; return Task.CompletedTask; }
}