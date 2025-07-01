using GameEngineService.Domain;
namespace GameEngineService.Application;

public interface IGameLogic { Game CreateGame(string gameId); Game MakeMove(string gameId, int position, string player); Game? GetGame(string gameId); }