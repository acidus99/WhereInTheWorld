using System;

using WhereInTheWorld.Models;

namespace WhereInTheWorld.Cgi
{
    public class GameRenderer
    {
        GameEngine Engine;
        TextWriter Output;
        GameState State;
        string Url;

        public GameRenderer(TextWriter output, GameEngine engine, string url)
        {
            Output = output;
            Engine = engine;
            Url = url;
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
                DrawSummary();
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
            Output.WriteLine($"=> {RouteOptions.PngUrl} See high resolution image");
            if(State.IsDebug)
            {
                Output.WriteLine($"Debug: Geography is {State.TargetCountry.Name}");
            }
            Output.WriteLine();
        }

        private void DrawTitle()
        {
            Output.WriteLine("# 🗺 Where in the World?");
            Output.WriteLine($"=> {RouteOptions.HelpUrl} How to play");
            Output.WriteLine($"=> {RouteOptions.FaqUrl} FAQ");
            Output.WriteLine($"## Puzzle #{State.PuzzleNumber} - {DateTime.Today.ToString("yyyy-MM-dd")}");
        }

        private void DrawGuesses()
        {
            int counter = 0;
            foreach(var guess in State.GuessResults)
            {
                counter++;
                Output.WriteLine($"* Guess {counter}. {guess.Country.Name} " +
                    $"| {guess.Distance} km | {BearingToEmoji(guess)} | {PercentAway(guess).ToString("P0")}"); 
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
            Output.WriteLine($"## You Win!");
            Output.WriteLine($"Congratulations! You correctly picked {State.TargetCountry.Name}!");
            Output.WriteLine("Come back tomorrow for another Where In The World puzzle!");
        }

        private void DrawLoseScreen()
        {
            Output.WriteLine($"## Bummer");
            Output.WriteLine($"Nice try, but the country was {State.TargetCountry.Name}.");
            Output.WriteLine("Come back tomorrow for another Where In The World puzzle!");
        }

        private void DrawSummary()
        {
            Output.WriteLine("## Share Game Summary");
            Output.WriteLine("Copy and share the summary of your game below on Station");
            Output.WriteLine("```game summary for copying");
            Output.WriteLine($"Where In The World? • Puzzle #{State.PuzzleNumber} • {DateTime.Today.ToString("yyyy-MM-dd")}");
            foreach (var guess in State.GuessResults)
            {
                Output.WriteLine($"{ClosenessGraph(guess)}{BearingToEmoji(guess)}");
            }
            Output.WriteLine(Url);
            Output.WriteLine("```");
        }

        private string ClosenessGraph(Guess guess)
        {
            double percent = PercentAway(guess);
            if (percent < .2)
            {
                return "❌❌❌❌❌";
            }
            else if (percent < .4)
            {
                return "✅❌❌❌❌";
            } else if (percent < .6)
            {
                return "✅✅❌❌❌";
            } else if (percent < .8)
            {
                return "✅✅✅❌❌";
            }
            else if (percent < .9)
            {
                return "✅✅✅✅❌";
            }
            else if (percent < 1)
            {
                return "✅✅✅✅🤏";
            }
            return "✅✅✅✅✅";
        }

        private string BearingToEmoji(Guess guess)
        {
            if(guess.IsCorrect)
            {
                return "🎉";
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

        private double PercentAway(Guess guess)
            => Convert.ToDouble(1f - (Convert.ToDouble(guess.Distance) / Convert.ToDouble(20000)));

    }
}

