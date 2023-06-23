using System;
using System.Security.Cryptography;
using Newtonsoft.Json;
using WhereInTheWorld.Models;

namespace WhereInTheWorld
{
	public class CountryData
	{
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
        public Country GetCountryForDay(DateTime date)
        {
            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes("hello" + date.DayOfYear.ToString()));
            var index = (bytes[0] % _countries.Count);
            return _countries.Values.ToArray()[index];
        }

        public bool IsValidCountry(string code)
            => _countries.ContainsKey(code.ToUpper());
    }
}

