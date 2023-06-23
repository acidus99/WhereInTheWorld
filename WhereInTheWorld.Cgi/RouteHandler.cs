using System;
using Gemini.Cgi;
using WhereInTheWorld;
namespace WhereInTheWorld.Cgi
{
	public static class RouteHandler
	{

		public static void PlayGame(CgiWrapper cgi)
        {
            var engine = new GameEngine(cgi.ExecutingPath);
            GameState state = engine.Play(ParseGuesses(cgi, engine));

            cgi.Success();
            GameRenderer renderer = new GameRenderer(cgi.Writer, engine, GetGameUrl(cgi));
            renderer.DrawState(state);
            Footer(cgi);
        }

        public static void ShowPng(CgiWrapper cgi)
        {
            var engine = new GameEngine(cgi.ExecutingPath);
            var country =  engine.PickCountryForPuzzle();
            cgi.Success("image/png");
            cgi.Out.Write(File.ReadAllBytes(Path.Combine(engine.ExecutionPath, $"assets/png/{country.Code.ToLower()}.png")));
        }

        static string GetGameUrl(CgiWrapper cgi)
            => $"gemini://{cgi.RequestUrl.Host}{cgi.RequestUrl.AbsolutePath}";

        static List<string> ParseGuesses(CgiWrapper cgi, GameEngine engine)
        {
            if (cgi.HasQuery)
            {
                return cgi.Query
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(x => engine.IsValidCountry(x))
                    .Select(x => x.ToUpper())
                    .Take(6)
                    .ToList();
            }
            return new List<string>();
        }

        static void Footer(CgiWrapper cgi)
        {
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine("--");
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 🌎 and ❤️ by Acidus");
        }
    }
}

