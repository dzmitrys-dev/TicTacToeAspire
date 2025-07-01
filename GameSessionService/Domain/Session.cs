namespace GameSessionService.Domain;

public class Session { public string SessionId { get; set; } = ""; public GameState? CurrentGameState { get; set; } public List<Move> MoveHistory { get; set; } = new(); }
