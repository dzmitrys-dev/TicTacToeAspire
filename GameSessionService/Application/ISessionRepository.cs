using GameSessionService.Domain;
namespace GameSessionService.Application;
public interface ISessionRepository { Task<Session?> FindByIdAsync(string sessionId); Task SaveAsync(Session session); }