using System;
namespace WhereInTheWorld.Models
{
    public class Guess
    {
        public bool IsCorrect { get; set; }

        public Country Country {get; set;}

        public double Bearing { get; set; }
        public double Distance { get; set; }
    }
}

