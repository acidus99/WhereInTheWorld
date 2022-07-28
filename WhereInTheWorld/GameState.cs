using System;


using WhereInTheWorld.Models;

namespace WhereInTheWorld
{
    public class GameState
    {
        public bool IsDebug { get; set; } = true;

        public List<string> InputGuesses { get; set; } = new List<string>();

        public List<Guess> GuessResults { get; set; } = new List<Guess>();
        public Country TargetCountry { get; set; } = null;

        public int PuzzleNumber { get; set; }

        //is the game complete?
        public bool IsComplete
            => IsWin || GuessResults.Count == 6;

        //did they win the game?
        public bool IsWin
            => GuessResults.Count > 0 && GuessResults.Last().IsCorrect;

    }
}

