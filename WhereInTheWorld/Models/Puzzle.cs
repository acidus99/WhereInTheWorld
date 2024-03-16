namespace WhereInTheWorld.Models;

public class Puzzle
{
    public static readonly DateTime InitialPuzzle = new DateTime(2022, 8, 1);

    public Country TargetCountry { get; set; }
    public int Number { get; set; }
    public DateTime Date { get; set; }

    public Puzzle(CountryData countries, DateTime date)
    {
        Date = date;
        Number = ComputeNumber(date);
        TargetCountry = countries.GetCountryForDay(date);
    }

    private int ComputeNumber(DateTime date)
        => Convert.ToInt32(Math.Floor(date.Subtract(InitialPuzzle).TotalDays)) + 1;
}
