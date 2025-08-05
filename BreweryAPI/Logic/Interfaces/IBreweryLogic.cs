using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Models;

namespace BreweryAPI.Logic.Interfaces
{
    public interface IBreweryLogic
    {
        Task<IEnumerable<Brewery>> GetAllBreweriesAsync();
        Task<Brewery?> GetBreweryByIdAsync(string id);
    }
}