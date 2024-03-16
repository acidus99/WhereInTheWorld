namespace WhereInTheWorld.Models;

public class Puzzle
{
    public static readonly DateTime InitialPuzzle = new DateTime(2022, 8, 1);

    public Country TargetCountry { get; set; }
    public int Number { get; set; }
    public DateTime Date { get; set; }

    public Puzzle(CountryData countries, int puzzleNumber)
    {
        Number = puzzleNumber;
        Date = NumberToDate(puzzleNumber);
        TargetCountry = countries.GetCountryForDay(Date);
    }

    public static DateTime NumberToDate(int puzzleNumber)
    {
        return InitialPuzzle.AddDays(puzzleNumber - 1);
    }
}
