using System;
using System.Security.Cryptography;

using BAMCIS.GIS;
using Newtonsoft.Json;
using WhereInTheWorld.Models;

namespace WhereInTheWorld
{
    //The only inputs to the game engine are the (validated) guesses the user has made,
    // and the country data.
    public class GameEngine
    {
        static readonly DateTime InitialPuzzle = new DateTime(2022, 8,1);

        readonly CountryData countries;

        public GameEngine(CountryData countryData)
        {
            countries = countryData;
        }

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
                TargetCountry = countries.GetCountryForDay(DateTime.Now),
                PuzzleNumber = ComputePuzzleNumber()
            };

            var targetGeo = new GeoCoordinate(state.TargetCountry.Latitude, state.TargetCountry.Longitude);

            //score the results
            foreach (string guessedCode in state.InputGuesses)
            {
                var country = countries[guessedCode];

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

    }
}

