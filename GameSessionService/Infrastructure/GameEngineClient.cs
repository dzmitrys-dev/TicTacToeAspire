using GameSessionService.Application;
using GameSessionService.Domain;
namespace GameSessionService.Infrastructure;

public class GameEngineClient : IGameEngineClient
{
    private readonly HttpClient _httpClient;
    public GameEngineClient(HttpClient httpClient) { _httpClient = httpClient; }

    public async Task<GameState?> CreateGameAsync(string gameId)
    {
        var response = await _httpClient.PostAsync($"api/games/{gameId}", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameState>();
    }

    public async Task<GameState?> MakeMoveAsync(string gameId, int position, string player)
    {
        var moveRequest = new { position, player };
        var response = await _httpClient.PostAsJsonAsync($"api/games/{gameId}/move", moveRequest);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GameState>();
    }
}