using GameSessionService.Domain;
namespace GameSessionService.Application;

public class SessionManager : ISessionManager
{
    private readonly ISessionRepository _sessionRepo;
    private readonly IGameEngineClient _gameEngineClient;
    private readonly IAutomatedPlayer _automatedPlayer;
    private readonly ILogger<SessionManager> _logger;

    public SessionManager(ISessionRepository sessionRepo, IGameEngineClient client, IAutomatedPlayer player, ILogger<SessionManager> logger)
    {
        _sessionRepo = sessionRepo; _gameEngineClient = client; _automatedPlayer = player; _logger = logger;
    }

    public async Task<Session> CreateSessionAsync()
    {
        var session = new Session { SessionId = Guid.NewGuid().ToString() };
        var gameState = await _gameEngineClient.CreateGameAsync(session.SessionId);
        if (gameState == null) throw new InvalidOperationException("Failed to create game in Game Engine.");
        session.CurrentGameState = gameState;
        await _sessionRepo.SaveAsync(session);
        return session;
    }

    public Session? GetSession(string sessionId) => _sessionRepo.FindByIdAsync(sessionId).Result;

    public async Task StartSimulationAsync(string sessionId)
    {
        var session = await _sessionRepo.FindByIdAsync(sessionId);
        if (session?.CurrentGameState == null) { _logger.LogError("Session not found or invalid."); return; }

        while (session.CurrentGameState.Status == "InProgress")
        {
            await Task.Delay(1000); // Delay for visualization
            try
            {
                var movePosition = _automatedPlayer.GetNextMove(session.CurrentGameState.Board);
                var currentPlayer = session.CurrentGameState.CurrentPlayer;

                var newGameState = await _gameEngineClient.MakeMoveAsync(sessionId, movePosition, currentPlayer);
                if (newGameState != null)
                {
                    session.CurrentGameState = newGameState;
                    session.MoveHistory.Add(new Move { Player = currentPlayer, Position = movePosition });
                    await _sessionRepo.SaveAsync(session);
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "Error during simulation for session {SessionId}.", sessionId); break; }
        }
        _logger.LogInformation("Simulation finished for session {SessionId} with status {Status}", sessionId, session.CurrentGameState.Status);
    }
}