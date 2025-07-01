using GameEngineService.Domain;
namespace GameEngineService.Application;

public class GameLogic : IGameLogic
{
    private readonly IGameRepository _repository;
    public GameLogic(IGameRepository repository) { _repository = repository; }
    public Game CreateGame(string gameId)
    {
        var game = new Game(gameId);
        _repository.SaveAsync(game);
        return game;
    }
    public Game? GetGame(string gameId) => _repository.FindByIdAsync(gameId).Result;
    public Game MakeMove(string gameId, int position, string player)
    {
        var game = _repository.FindByIdAsync(gameId).Result ?? throw new InvalidOperationException("Game not found.");
        if (game.Status != GameStatus.InProgress) throw new InvalidOperationException("Game is already over.");
        if (game.CurrentPlayer != player) throw new InvalidOperationException($"It's not player {player}'s turn.");
        if (position < 0 || position >= 9 || game.Board[position] != " ") throw new InvalidOperationException("Invalid move.");

        game.ApplyMove(position, player);
        _repository.SaveAsync(game);
        return game;
    }
}