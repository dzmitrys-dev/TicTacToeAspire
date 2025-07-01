using Aspire.Hosting;
using Aspire.Hosting.Testing;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TicTacToeAspire.Tests;

[TestFixture]
public class IntegrationTests
{
    private DistributedApplication _app = null!;

    [OneTimeSetUp]
    public async Task FixtureSetup()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TicTacToeAspire_AppHost>();
        _app = await appHost.BuildAsync();
        await _app.StartAsync();
    }

    [OneTimeTearDown]
    public async Task FixtureTearDown()
    {
        await _app.StopAsync();
    }

    [Test, Order(1)]
    public async Task FullGame_ViaApiGateway_ShouldCompleteSuccessfully()
    {
        // Arrange
        var gatewayClient = _app.CreateHttpClient("apigateway");

        // Act
        var createResponse = await gatewayClient.PostAsync("/api/sessions", null);
        createResponse.EnsureSuccessStatusCode();
        var session = await createResponse.Content.ReadFromJsonAsync<Session>();
        Assert.That(session, Is.Not.Null);

        var simulateResponse = await gatewayClient.PostAsync($"/api/sessions/{session.SessionId}/simulate", null);
        simulateResponse.EnsureSuccessStatusCode();

        while (session?.CurrentGameState?.Status == "InProgress")
        {
            await Task.Delay(1500);
            session = await gatewayClient.GetFromJsonAsync<Session>($"/api/sessions/{session.SessionId}");
        }

        // Assert
        Assert.That(session?.CurrentGameState?.Status, Is.Not.EqualTo("InProgress"));
        var validStatuses = new[] { "CrossesWins", "CirclesWins", "Draw" };
        Assert.That(validStatuses, Does.Contain(session?.CurrentGameState?.Status));
        Assert.That(session?.MoveHistory, Is.Not.Empty);
    }

    [Test, Order(2)]
    public async Task MakeMove_ConcurrentRequests_ShouldBeHandledCorrectly()
    {
        // Arrange
        var gatewayClient = _app.CreateHttpClient("apigateway");
        var gameId = $"concurrent-test-{Guid.NewGuid()}";

        await gatewayClient.PostAsync($"/api/games/{gameId}", null);

        // Act
        var move1 = gatewayClient.PostAsJsonAsync($"/api/games/{gameId}/move", new MoveRequest(0, "🤓"));
        var move2 = gatewayClient.PostAsJsonAsync($"/api/games/{gameId}/move", new MoveRequest(1, "😅"));
        var move3 = gatewayClient.PostAsJsonAsync($"/api/games/{gameId}/move", new MoveRequest(2, "🤓"));

        await Task.WhenAll(move1, move2, move3);

        // Assert
        var finalStateResponse = await gatewayClient.GetAsync($"/api/games/{gameId}");
        finalStateResponse.EnsureSuccessStatusCode();
        var finalState = await finalStateResponse.Content.ReadFromJsonAsync<GameState>();

        Assert.That(finalState?.Board[0], Is.EqualTo("🤓"));
        Assert.That(finalState?.Board[1], Is.EqualTo("😅"));
        Assert.That(finalState?.Board[2], Is.EqualTo("🤓"));
        Assert.That(finalState?.CurrentPlayer, Is.EqualTo("😅"));
    }

    // DTO models
    private record Session([property: JsonPropertyName("sessionId")] string SessionId, [property: JsonPropertyName("currentGameState")] GameState? CurrentGameState, [property: JsonPropertyName("moveHistory")] List<object> MoveHistory);
    private record GameState([property: JsonPropertyName("board")] string[] Board, [property: JsonPropertyName("status")] string Status, [property: JsonPropertyName("currentPlayer")] string CurrentPlayer);
    private record MoveRequest(int Position, string Player);
}