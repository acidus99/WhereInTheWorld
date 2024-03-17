using System.Security.Cryptography;
using Newtonsoft.Json;
using WhereInTheWorld.Models;

namespace WhereInTheWorld;

public class CountryData
{
    //After this epoc, we use a different algorithm to determine the country to use for a day's puzzle.
    static readonly DateTime SecondAlgorithmEpoc = new DateTime(2024, 3, 16);

    Dictionary<string, Country> _countries;

    public CountryData(string filename)
        {
        var json = File.ReadAllText(filename);
        var countries = JsonConvert.DeserializeObject<Country[]>(json);

        if (countries == null)
        {
            throw new ApplicationException("Could not deserialize country data!");
        }

        _countries = countries.Where(x => x.HasMap).ToDictionary(x => x.Code, x => x);
    }

    public Country this[string code]
    {
        get => _countries[code];
    }

    public IEnumerable<Country> Countries
        => _countries.Values;

    // Get the country for a specific day's puzzle
    public Country GetCountryForPuzzle(DateTime date, int puzzleNumber)
        => (date <= SecondAlgorithmEpoc) ?
            GetCountryOriginalAlgorithm(date) :
            GetCountrySecondAlgorithm(puzzleNumber);

    /// <summary>
    /// Original algorithm used to determine the country for a day's puzzle.
    /// Because this used "Day of Year" than puzzle #1 and puzzle #1 366 would have the same answer.
    /// In fact, there was a pattern of countries for an entire year, that was used every year.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private Country GetCountryOriginalAlgorithm(DateTime date)
    {
        var md5 = MD5.Create();
        var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes("hello" + date.DayOfYear.ToString()));
        var index = (bytes[0] % _countries.Count);
        return _countries.Values.ToArray()[index];
    }

    /// <summary>
    /// Newer algorithm to find a country. Since this uses the always increasing puzzle number as input
    /// there will not be repeating patterns.
    /// </summary>
    /// <param name="puzzleNumber"></param>
    /// <returns></returns>
    private Country GetCountrySecondAlgorithm(int puzzleNumber)
    {
        var md5 = MD5.Create();
        var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes("hello" + puzzleNumber));
        var index = (bytes[0] % _countries.Count);
        return _countries.Values.ToArray()[index];
    }


    public bool IsValidCountry(string code)
        => _countries.ContainsKey(code.ToUpper());
}

