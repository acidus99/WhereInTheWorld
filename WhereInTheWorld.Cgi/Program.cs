using System;


using Gemini.Cgi;
using WhereInTheWorld.Models;

namespace WhereInTheWorld.Cgi
{
    class Program
    {
        static GameEngine Engine;
        static GameState State;

        static void Main(string[] args)
        {
            using (var cgi = new CgiWrapper())
            {
                Engine = new GameEngine(cgi.ExecutingPath);
                State = new GameState
                {
                    InputGuesses = ParseGuesses(cgi)
                };
                Engine.Play(State);

                //now output results

                Uri url = cgi.RequestUrl;
                

                cgi.Success();
                GameRenderer renderer = new GameRenderer(cgi.Writer, Engine, GetGameUrl(cgi));
                renderer.DrawState(State);
            }
        }

        static string GetGameUrl(CgiWrapper cgi)
            => $"gemini://{cgi.RequestUrl.Host}{cgi.RequestUrl.AbsolutePath}";

        static List<string> ParseGuesses(CgiWrapper cgi)
        {
            if (cgi.HasQuery)
            {
                return cgi.Query
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(x => Engine.IsValidCountry(x))
                    .Select(x => x.ToUpper())
                    .Take(6)
                    .ToList();
            }
            return new List<string>();
        }

    }
}
