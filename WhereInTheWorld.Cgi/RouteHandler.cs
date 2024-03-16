using Gemini.Cgi;
using System.Text.RegularExpressions;
using WhereInTheWorld.Models;

namespace WhereInTheWorld.Cgi;

public static class RouteHandler
{
    static readonly Regex PuzzleRegex = new Regex(@"\/game\/(\d+)\/", RegexOptions.Compiled);

    public static void PlayToday(CgiWrapper cgi)
    {
        //convert today's date to puzzle number
        int puzzleNumber = Puzzle.DateToNumber(DateTime.UtcNow.Date);
        cgi.Redirect(RouteOptions.PlayUrl(puzzleNumber));
    }

    public static void PlayGame(CgiWrapper cgi)
    {
        //load up our country data
        CountryData countries = LoadCountryData(cgi.ExecutingPath);
        Puzzle puzzle = ParsePuzzle(cgi, countries);

        var engine = new GameEngine(countries, puzzle);

        //parse and validate the guess from the player
        List<string> guesses = ParseGuesses(cgi, countries);

        //execute the game and determine the final state;
        GameState state = engine.Play(guesses);

        cgi.Success();
        GameRenderer renderer = new GameRenderer(cgi.Writer, countries, GetAssetsPath(cgi.ExecutingPath), GetGameUrl(cgi));

        //render the state to the player
        renderer.DrawState(state);
        Footer(cgi);
    }

    public static void ShowPng(CgiWrapper cgi)
    {
        CountryData countries = LoadCountryData(cgi.ExecutingPath);
        Puzzle puzzle = ParsePuzzle(cgi, countries);

        cgi.Success("image/png");
        cgi.Out.Write(File.ReadAllBytes($"{GetAssetsPath(cgi.ExecutingPath)}png/{puzzle.TargetCountry.Code.ToLower()}.png"));
    }

    static string GetGameUrl(CgiWrapper cgi)
        => $"gemini://{cgi.RequestUrl.Host}{cgi.RequestUrl.AbsolutePath}";

    static List<string> ParseGuesses(CgiWrapper cgi, CountryData countries)
    {
        if (cgi.HasQuery)
        {
            return cgi.Query
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => countries.IsValidCountry(x))
                .Select(x => x.ToUpper())
                .Take(6)
                .ToList();
        }
        return new List<string>();
    }

    static CountryData LoadCountryData(string rootPath)
        => new CountryData($"{rootPath}/data/countries.json");

    static Puzzle ParsePuzzle(CgiWrapper cgi, CountryData countries)
    {
        int puzzleNumber = ParsePuzzleNumber(cgi.RequestUrl);
        var puzzle = new Puzzle(countries, puzzleNumber);

        //don't allow access to future puzzles
        if(puzzle.Date.Date > DateTime.UtcNow.Date)
        {
            throw new ApplicationException("Puzzle is in the future and cannot be played now");
        }
        return puzzle;
    }

    static int ParsePuzzleNumber(Uri url)
    {
        var match = PuzzleRegex.Match(url.AbsolutePath);

        if (!match.Success)
        {
            throw new ApplicationException("Invalid Game URL.");
        }
        return Convert.ToInt32(match.Groups[1].Value);
    }

    static string GetAssetsPath(string rootPath)
        => $"{rootPath}/assets/";

    static void Footer(CgiWrapper cgi)
    {
        cgi.Writer.WriteLine();
        cgi.Writer.WriteLine("--");
        cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 🌎 and ❤️ by Acidus");
    }
}

