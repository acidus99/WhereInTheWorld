using WhereInTheWorld.Models;
using System.Web;

namespace WhereInTheWorld.Cgi;

public class GameRenderer
{
    readonly CountryData Countries;
    readonly TextWriter Output;
    readonly string AssetsPath;
    readonly string PlayUrl;

    public GameRenderer(TextWriter output, CountryData countries, string assetsPath, string url)
    {
        Output = output;
        Countries = countries;
        AssetsPath = assetsPath;
        PlayUrl = url;
    }

    public void DrawState(GameState state)
    {
        DrawTitle(state.Puzzle);
        DrawCountry(state);
        DrawGuesses(state.GuessResults);
        if (!state.IsComplete)
        {
            DrawInputOptions(state);
        }
        else
        {
            if (state.IsWin)
            {
                DrawWinScreen(state);
            }
            else
            {
                DrawLoseScreen(state);
            }
            DrawSummary(state);
            DrawPuzzleLinks(state.Puzzle);
        }
    }

    private string AsciiMapForCountry(Country country)
    {
        try
        {
            return File.ReadAllText($"{AssetsPath}{country.Code.ToLower()}.60.txt");
        }
        catch (Exception)
        {
            return "";
        }
    }

    private void DrawCountry(GameState state)
    {
        Output.WriteLine("``` Geoography you are guessing");
        Output.Write(AsciiMapForCountry(state.Puzzle.TargetCountry));
        Output.WriteLine("```");
        Output.WriteLine($"=> {RouteOptions.PngUrl(state.Puzzle.Number)} See high resolution image");
        if (state.IsDebug)
        {
            Output.WriteLine($"Debug: Answer is {state.Puzzle.TargetCountry.Name}");
        }
        Output.WriteLine();
    }

    private void DrawTitle(Puzzle puzzle)
    {
        Output.WriteLine("# 🗺 Where in the World?");
        Output.WriteLine($"## Puzzle #{puzzle.Number} - {puzzle.Date.ToString("yyyy-MM-dd")}");
    }

    private void DrawGuesses(List<Guess> guesses)
    {
        int counter = 0;
        foreach (var guess in guesses)
        {
            counter++;
            Output.WriteLine($"* Guess {counter}. {guess.Country.Name} " +
                $"• {guess.Distance} km • {BearingToEmoji(guess)} • {PercentAway(guess).ToString("P0")}");
        }
        Output.WriteLine();
    }

    private void DrawInputOptions(GameState state)
    {
        Output.WriteLine("## Guess a Country");
        string previous = "A";

        string prevInput = String.Join(',', state.InputGuesses);

        foreach (var country in Countries.Countries
            .Where(x => !state.InputGuesses.Contains(x.Code))
            .OrderBy(x => x.Name))
        {
            if (country.Name[0] != previous[0])
            {
                Output.WriteLine();
            }
            previous = country.Name;
            Output.WriteLine($"=> {RouteOptions.PlayUrl(state.Puzzle.Number)}?{prevInput},{country.Code} {country.Name}");
        }
    }

    private void DrawWinScreen(GameState state)
    {
        Output.WriteLine($"## You Win!");
        Output.WriteLine($"Congratulations! You correctly picked {state.Puzzle.TargetCountry.Name}!");
        Output.WriteLine($"=> /cgi-bin/wp.cgi/view?{HttpUtility.UrlEncode(state.Puzzle.TargetCountry.Name)} Read Wikipedia article about {state.Puzzle.TargetCountry.Name}");
        Output.WriteLine();
    }

    private void DrawLoseScreen(GameState state)
    {
        Output.WriteLine($"## Bummer");
        Output.WriteLine($"Nice try, but the country was {state.Puzzle.TargetCountry.Name}.");
        Output.WriteLine($"=> /cgi-bin/wp.cgi/view?{HttpUtility.UrlEncode(state.Puzzle.TargetCountry.Name)} Read Wikipedia article about {state.Puzzle.TargetCountry.Name}");
        Output.WriteLine();
    }

    private void DrawPuzzleLinks(Puzzle puzzle)
    {
        int? nextNumber = Puzzle.IsValidPuzzle(puzzle.Number + 1) ? puzzle.Number + 1 : null;
        int? prevNumber = Puzzle.IsValidPuzzle(puzzle.Number - 1) ? puzzle.Number - 1 : null;

        if (nextNumber.HasValue)
        {
            Output.WriteLine($"=> {RouteOptions.PlayUrl(nextNumber.Value)} Play next puzzle #{nextNumber.Value}");
        }
        else
        {
            Output.WriteLine("Come back tomorrow for another Where In The World puzzle!");
        }

        if (prevNumber.HasValue)
        {
            Output.WriteLine($"=> {RouteOptions.PlayUrl(prevNumber.Value)} Play previous puzzle #{prevNumber.Value}");
        }
        Output.WriteLine();
    }

    private void DrawSummary(GameState state)
    {
        Output.WriteLine("## Share Game Summary");
        Output.WriteLine("Copy and share the summary of your game below on Station");
        Output.WriteLine("```game summary for copying");
        Output.WriteLine($"Where In The World? • Puzzle #{state.Puzzle.Number} • {state.Puzzle.Date.ToString("yyyy-MM-dd")}");
        foreach (var guess in state.GuessResults)
        {
            Output.WriteLine($"{ClosenessGraph(guess)}{BearingToEmoji(guess)}");
        }
        Output.WriteLine(PlayUrl);
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
        }
        else if (percent < .6)
        {
            return "✅✅❌❌❌";
        }
        else if (percent < .8)
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
        if (guess.IsCorrect)
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
    {
        var precise = Convert.ToDouble(1f - (Convert.ToDouble(guess.Distance) / Convert.ToDouble(20000)));
        //don't allow lower numbers to be 0%
        if (precise <= 0.009)
        {
            return 0.01;
        }
        //truncate to make percentage whole numbers. This prevents "rounding up" by later string functions
        return Math.Truncate(precise * 100) / 100;
    }
}
