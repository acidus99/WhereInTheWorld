using System;


using WhereInTheWorld.Models;

namespace WhereInTheWorld
{
    public class GameState
    {
        public bool IsDebug { get; set; } = false;

        public required List<string> InputGuesses { get; set; }

        public List<Guess> GuessResults { get; set; } = new List<Guess>();
        public required Country TargetCountry { get; set; }

        public required int PuzzleNumber { get; set; }

        //is the game complete?
        public bool IsComplete
            => IsWin || GuessResults.Count == 6;

        //did they win the game?
        public bool IsWin
            => GuessResults.Count > 0 && GuessResults.Last().IsCorrect;
    }
}

