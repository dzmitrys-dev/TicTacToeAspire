using NUnit.Framework;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TicTacToeAspire.Tests;

[TestFixture]
public class FullSimulationTests
{
    /// <summary>
    /// This test launches the entire distributed application in-memory,
    //  including all services, and tests the full game simulation flow.
    /// </summary>
    [Test]
    public async Task CreateAndSimulate_FullGame_ShouldCompleteSuccessfully()
    {
        // Arrange: Build and start the distributed application from the AppHost.
        // This automatically handles service discovery, environment variables, and ports.
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.TicTacToeAspire_AppHost>();
        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        // Act:
        // 1. Get an HttpClient to communicate with the GameSessionService.
        // The service name "gamesession" is pulled from your AppHost configuration.
        var sessionClient = app.CreateHttpClient("gamesession");

        // 2. Create a new session by calling the POST api/sessions endpoint.
        var createResponse = await sessionClient.PostAsync("api/sessions", null);
        createResponse.EnsureSuccessStatusCode();
        var session = await createResponse.Content.ReadFromJsonAsync<Session>();
        Assert.That(session, Is.Not.Null);

        // 3. Start the automated simulation.
        var simulateResponse = await sessionClient.PostAsync($"api/sessions/{session.SessionId}/simulate", null);
        simulateResponse.EnsureSuccessStatusCode();

        // 4. Poll the session status until the game is no longer "InProgress".
        while (session?.CurrentGameState?.Status == "InProgress")
        {
            // Wait for 1.5 seconds to give the server time to process a move.
            await Task.Delay(1500);
            session = await sessionClient.GetFromJsonAsync<Session>($"api/sessions/{session.SessionId}");
        }

        // Assert:
        // 1. The final game status should not be "InProgress".
        Assert.That(session?.CurrentGameState?.Status, Is.Not.EqualTo("InProgress"));

        // 2. The game should have a valid conclusion (win or draw).
        var validStatuses = new[] { "CrossesWins", "CirclesWins", "Draw" };
        Assert.That(validStatuses, Does.Contain(session?.CurrentGameState?.Status));

        // 3. The move history should contain moves. A game needs at least 5 moves to conclude.
        Assert.That(session?.MoveHistory, Is.Not.Empty);
        Assert.That(session?.MoveHistory.Count, Is.GreaterThanOrEqualTo(5));

        // Cleanup: The 'await using' statement automatically stops the application.
    }

    // DTO models to match the structure of the API response for deserialization.
    private class Session
    {
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; } = string.Empty;

        [JsonPropertyName("currentGameState")]
        public GameState? CurrentGameState { get; set; }

        [JsonPropertyName("moveHistory")]
        public List<object> MoveHistory { get; set; } = new();
    }

    private class GameState
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}