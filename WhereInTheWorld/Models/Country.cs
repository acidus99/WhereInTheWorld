using System;
using Newtonsoft.Json;

namespace WhereInTheWorld.Models
{
    public class Country
    {
        public string Code { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Name { get; set; }

        public bool HasMap { get; set; } = true;
    }
}

