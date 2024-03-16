using BAMCIS.GIS;
using WhereInTheWorld.Models;

namespace WhereInTheWorld;

//The only inputs to the game engine are the (validated) guesses the user has made,
// and the country data.
public class GameEngine
{
    readonly CountryData countries;
    readonly Puzzle Puzzle;

    public GameEngine(CountryData countryData, Puzzle puzzle)
    {
        countries = countryData;
        Puzzle = puzzle;
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
            Puzzle = Puzzle
        };

        var targetGeo = new GeoCoordinate(Puzzle.TargetCountry.Latitude, Puzzle.TargetCountry.Longitude);

        //score the results
        foreach (string guessedCode in state.InputGuesses)
        {
            var country = countries[guessedCode];

            GeoCoordinate GuessGeo = new GeoCoordinate(country.Latitude, country.Longitude);
            bool isCorrect = (guessedCode == Puzzle.TargetCountry.Code);

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
}
