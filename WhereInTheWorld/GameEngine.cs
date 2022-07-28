using System;
using System.Security.Cryptography;

using BAMCIS.GIS;
using Newtonsoft.Json;
using WhereInTheWorld.Models;

namespace WhereInTheWorld
{
    public class GameEngine
    {
        static readonly DateTime InitialPuzzle = DateTime.Today;

        public Dictionary<string, Country> Countries { get; internal set; }

        /// <summary>
        /// Where is the game executing? used to load data and graphics
        /// </summary>
        public string ExecutionPath { get; private set; }

        public GameEngine(string execPath)
        {
            ExecutionPath = execPath;
            LoadData();
        }

        public bool IsValidCountry(string code)
            => Countries.ContainsKey(code.ToUpper());

        public void Play(GameState state)
        {
            //select target country for the game
            //currently derived from date, so consistent and changes once a day
            var targetCountry = PickTargetCountry();
            state.TargetCountry = targetCountry;
            state.PuzzleNumber = ComputePuzzleNumber();

            var targetGeo = new GeoCoordinate(targetCountry.Latitude, targetCountry.Longitude);

            //score the results
            foreach (string guessedCode in state.InputGuesses)
            {
                var country = Countries[guessedCode];

                GeoCoordinate GuessGeo = new GeoCoordinate(country.Latitude, country.Longitude);
                bool isCorrent = (guessedCode == targetCountry.Code);

                state.GuessResults.Add(new Guess
                {
                    Country = country,
                    IsCorrect = isCorrent,
                    Bearing = GuessGeo.RhumbBearingTo(targetGeo),
                    Distance = Convert.ToInt32(GuessGeo.DistanceTo(targetGeo, DistanceType.KILOMETERS))
                });

                if (isCorrent)
                {
                    break;
                }
            }
        }

        private int ComputePuzzleNumber()
            => Convert.ToInt32(Math.Floor(DateTime.Now.Subtract(InitialPuzzle).TotalDays)) + 1;
        

        private void LoadData()
        {
            var json = File.ReadAllText(Path.Combine(ExecutionPath, "data/countries.json"));

            Countries = JsonConvert.DeserializeObject<Country[]>(json)
                ?.Where(x=>x.HasMap).ToDictionary(x => x.Code, x => x);
        }

        private Country PickTargetCountry()
        {
            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes("hello" + DateTime.Now.DayOfYear.ToString()));
            var index = (bytes[0] % Countries.Count);
            return Countries.Values.ToArray()[index];
        }
    }
}

