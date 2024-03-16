namespace WhereInTheWorld.Models;

public class Guess
{
    public required bool IsCorrect { get; set; }

    public required Country Country {get; set;}

    public required double Bearing { get; set; }
    public required double Distance { get; set; }
}

