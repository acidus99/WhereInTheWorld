using System;

using WhereInTheWorld.Models;

namespace WhereInTheWorld.Cgi
{
    public class GameRenderer
    {
        GameEngine Engine;
        TextWriter Output;
        GameState State;

        public GameRenderer(TextWriter output, GameEngine engine)
        {
            Output = output;
            Engine = engine;
        }

        public void DrawState(GameState state)
        {
            State = state;

            foreach(var country in Engine.Countries.Values)
            {
                if(AsciiMapForCountry(country) == "")
                {
                    Output.WriteLine($"* {country.Code} {country.Name}");
                }

            }

            DrawTitle();
            DrawCountry();
            DrawGuesses();
            if (!state.IsComplete)
            {
                DrawInputOptions();
            } else
            {
                if(state.IsWin)
                {
                    DrawWinScreen();
                } else
                {
                    DrawLoseScreen();
                }
            }
        }

        private string AsciiMapForCountry(Country country)
        {
            try
            {
                return File.ReadAllText(Path.Combine(Engine.ExecutionPath, $"assets/{country.Code.ToLower()}.60.txt"));
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void DrawCountry()
        {
            Output.WriteLine("``` Geoography you are guessing");
            Output.Write(AsciiMapForCountry(State.TargetCountry));
            Output.WriteLine("```");
            if(State.IsDebug)
            {
                Output.WriteLine($"Debug: Geography is {State.TargetCountry.Name}");
            }
            Output.WriteLine();
        }

        private void DrawTitle()
        {
            Output.WriteLine("# 🗺 Where in the World?");
            Output.WriteLine($"## Puzzle #{State.PuzzleNumber} - {DateTime.Today.ToString("yyyy-MM-dd")}");
            Output.WriteLine();
        }

        private void DrawGuesses()
        {
            int counter = 0;
            foreach(var guess in State.GuessResults)
            {
                counter++;
                Output.WriteLine($"* Guess {counter}. {guess.Country.Name} " +
                    $"| {guess.Distance} km | {BearingToEmoji(guess)} | {PercentAway(guess)}"); 
            }
        }

        private void DrawInputOptions()
        {
            Output.WriteLine("## Guess a Country");
            string previous = "A";

            string prevInput = String.Join(',', State.InputGuesses);

            foreach (var country in Engine.Countries.Values
                .Where(x => !State.InputGuesses.Contains(x.Code))
                .OrderBy(x => x.Name))
            {
                if (country.Name[0] != previous[0])
                {
                    Output.WriteLine();
                }
                previous = country.Name;
                Output.WriteLine($"=> ?{prevInput},{country.Code} {country.Name}");
            }
        }

        private void DrawWinScreen()
        {
            Output.WriteLine($"Congradulations! You correctly picked {State.TargetCountry.Name}!");
            Output.WriteLine("Come back tomorrow for another puzzle.");
        }

        private void DrawLoseScreen()
        {
            Output.WriteLine($"Nope! The Country was {State.TargetCountry.Name}.");
            Output.WriteLine("Come back tomorrow for another puzzle.");
        }

        private string DrawSummary()
        {
            return "";
        }

        private string BearingToEmoji(Guess guess)
        {
            if(guess.IsCorrect)
            {
                return "🎉🎉🎉";
            }

            //normalize bearing into 45 degree points;
            int bearing = Convert.ToInt32(Math.Round(guess.Bearing / 45));
            switch (bearing)
            {
                case 1:
                    return "↗️";

                case 2:
                    return "➡️";

                case 3:
                    return "↘️";

                case 4:
                    return "⬇️";

                case 5:
                    return "↙️";

                case 6:
                    return "⬅️";

                case 7:
                    return "↖️";

                default: //0 and 8
                    return "⬆️";
            }
        }

        private string PercentAway(Guess guess)
            => Convert.ToDouble(1f - (Convert.ToDouble(guess.Distance) / Convert.ToDouble(20000))).ToString("P0");

    }
}

