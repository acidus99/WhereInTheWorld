using System;
using Newtonsoft.Json;

namespace WhereInTheWorld.Models
{
    public class Country
    {
        public required string Code { get; set; }

        public required double Latitude { get; set; }

        public required double Longitude { get; set; }

        public required string Name { get; set; }

        public bool HasMap { get; set; } = true;
    }
}

