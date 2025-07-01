namespace GameSessionService.Application;
public class RandomPlayer : IAutomatedPlayer
{
    public int GetNextMove(string[] board)
    {
        var availableMoves = new List<int>();
        for (int i = 0; i < board.Length; i++) { if (board[i] == " ") availableMoves.Add(i); }
        if (availableMoves.Count == 0) throw new InvalidOperationException("No available moves.");
        return availableMoves[new Random().Next(availableMoves.Count)];
    }
}