using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Models;

namespace BreweryAPI.Repositories.Interfaces
{
    public interface IBreweryRepository
    {
        Task<IEnumerable<Brewery>> GetAllBreweriesAsync();
        Task<Brewery?> GetBreweryByIdAsync(string id);
        Task<IEnumerable<Brewery>> SearchAsync(string query);
        Task<IEnumerable<Brewery>> GetByCityAsync(string city);
        Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query);
    }
}