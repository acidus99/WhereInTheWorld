using System;
using Gemini.Cgi;
using WhereInTheWorld;

namespace WhereInTheWorld.Cgi
{
	public static class RouteHandler
	{
		public static void PlayGame(CgiWrapper cgi)
        {
            //load up our country data
            CountryData countries = LoadCountryData(cgi.ExecutingPath);
            var engine = new GameEngine(countries); ;

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
            var country =  countries.GetCountryForDay(DateTime.Now);
            cgi.Success("image/png");
            cgi.Out.Write(File.ReadAllBytes($"{GetAssetsPath(cgi.ExecutingPath)}png/{country.Code.ToLower()}.png"));
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
            => new CountryData($"{rootPath}data/countries.json");

        static string GetAssetsPath(string rootPath)
            => $"{rootPath}assets/";

        static void Footer(CgiWrapper cgi)
        {
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine("--");
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 🌎 and ❤️ by Acidus");
        }
    }
}

