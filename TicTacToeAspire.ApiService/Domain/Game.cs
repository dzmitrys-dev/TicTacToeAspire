using System.Text.Json.Serialization;

namespace GameEngineService.Domain;

public class Game
{
    public string GameId { get; init; }
    public string[] Board { get; private set; } = new string[9];
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GameStatus Status { get; private set; } = GameStatus.InProgress;
    public string CurrentPlayer { get; private set; } = Player.Crosses;

    public Game(string gameId)
    {
        GameId = gameId;
        for (int i = 0; i < Board.Length; i++) Board[i] = " ";
    }

    public void ApplyMove(int position, string player)
    {
        Board[position] = player;
        CurrentPlayer = (player == Player.Crosses) ? Player.Circles : Player.Crosses;
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        int[][] wins = {
            new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // Rows
            new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // Columns
            new[] {0, 4, 8}, new[] {2, 4, 6}                  // Diagonals
        };

        foreach (var win in wins)
        {
            if (Board[win[0]] != " " && Board[win[0]] == Board[win[1]] && Board[win[1]] == Board[win[2]])
            {
                Status = Board[win[0]] == Player.Crosses ? GameStatus.CrossesWins : GameStatus.CirclesWins;
                return;
            }
        }

        if (Board.All(c => c != " "))
        {
            Status = GameStatus.Draw;
        }
    }
}