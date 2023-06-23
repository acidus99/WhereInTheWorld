using System;
using System.Security.Cryptography;

using BAMCIS.GIS;
using Newtonsoft.Json;
using WhereInTheWorld.Models;

namespace WhereInTheWorld
{
    public class GameEngine
    {
        static readonly DateTime InitialPuzzle = new DateTime(2022, 8,1);

        public Dictionary<string, Country> Countries { get; internal set; }

        /// <summary>
        /// Where is the game executing? used to load data and graphics
        /// </summary>
        public string ExecutionPath { get; private set; }

        public GameEngine(string execPath)
        {
            ExecutionPath = execPath;
            Countries = LoadCountryData();
        }

        public bool IsValidCountry(string code)
            => Countries.ContainsKey(code.ToUpper());

        /// <summary>
        /// Given a set of guess, run the game and return the game state
        /// </summary>
        /// <param name="guesses">list of country codes the player has guessed</param>
        /// <returns>the state of the game</returns>
        public GameState Play(List<string> guesses)
        {
            var state = new GameState
            {
                InputGuesses = guesses,
                TargetCountry = PickCountryForPuzzle(),
                PuzzleNumber = ComputePuzzleNumber()
            };

            var targetGeo = new GeoCoordinate(state.TargetCountry.Latitude, state.TargetCountry.Longitude);

            //score the results
            foreach (string guessedCode in state.InputGuesses)
            {
                var country = Countries[guessedCode];

                GeoCoordinate GuessGeo = new GeoCoordinate(country.Latitude, country.Longitude);
                bool isCorrect = (guessedCode == state.TargetCountry.Code);

                state.GuessResults.Add(new Guess
                {
                    Country = country,
                    IsCorrect = isCorrect,
                    Bearing = GuessGeo.RhumbBearingTo(targetGeo),
                    Distance = Convert.ToInt32(GuessGeo.DistanceTo(targetGeo, DistanceType.KILOMETERS))
                });

                if (isCorrect)
                {
                    break;
                }
            }

            return state;
        }

        private int ComputePuzzleNumber()
            => Convert.ToInt32(Math.Floor(DateTime.Now.Subtract(InitialPuzzle).TotalDays)) + 1;

        private Dictionary<string, Country> LoadCountryData()
        {
            var json = File.ReadAllText(Path.Combine(ExecutionPath, "data/countries.json"));
            var countries = JsonConvert.DeserializeObject<Country[]>(json);

            if(countries == null)
            {
                throw new ApplicationException("Could not deserialize country data");
            }
            return countries.Where(x => x.HasMap).ToDictionary(x => x.Code, x => x);
        }

        public Country PickCountryForPuzzle()
        {
            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes("hello" + DateTime.Now.DayOfYear.ToString()));
            var index = (bytes[0] % Countries.Count);
            return Countries.Values.ToArray()[index];
        }
    }
}

