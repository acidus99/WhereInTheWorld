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

    public static int DateToNumber(DateTime date)
      => Convert.ToInt32(Math.Floor(date.Subtract(InitialPuzzle).TotalDays)) + 1;

    public static DateTime NumberToDate(int puzzleNumber)
        => InitialPuzzle.AddDays(puzzleNumber - 1);
}
