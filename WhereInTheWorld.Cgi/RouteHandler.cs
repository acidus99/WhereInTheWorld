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
            var state = new GameState
            {
                InputGuesses = ParseGuesses(cgi, engine)
            };
            engine.Play(state);

            cgi.Success();
            GameRenderer renderer = new GameRenderer(cgi.Writer, engine, GetGameUrl(cgi));
            renderer.DrawState(state);
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


    }
}

