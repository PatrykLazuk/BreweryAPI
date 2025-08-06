using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Models;

namespace BreweryAPI.Logic.Interfaces
{
    public interface IBreweryLogic
    {
        Task<PagedResult<Brewery>> GetAllBreweriesAsync(
            string? search, string? city, string? sortBy, double? userLat, double? userLng, int page, int pageSize);
        Task<Brewery?> GetBreweryByIdAsync(string id);
        Task<IEnumerable<Brewery>> SearchAsync(string query);
        Task<IEnumerable<Brewery>> GetByCityAsync(string city);
        Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query);
    }
}