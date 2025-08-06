using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreweryAPI.Models
{
    public class BreweryV2Dto
    {
        public Brewery? Brewery { get; set; }
        public string Info { get; set; } = string.Empty;
    }
}