using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreweryAPI.Models
{
    public class Brewery
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}