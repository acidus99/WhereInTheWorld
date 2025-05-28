using WhereInTheWorld.Models;

namespace WhereInTheWorld;

public class GameState
{
    public bool IsDebug { get; set; } = false;

    public required List<string> InputGuesses { get; set; }

    public required Puzzle Puzzle { get; set; }
    
    public bool IsForfeit { get; set; } = false;

    public List<Guess> GuessResults { get; set; } = new List<Guess>();

    //is the game complete?
    public bool IsComplete
        => IsWin || IsForfeit || GuessResults.Count == 6;

    //did they win the game?
    public bool IsWin
        => GuessResults.Count > 0 && GuessResults.Last().IsCorrect;
}