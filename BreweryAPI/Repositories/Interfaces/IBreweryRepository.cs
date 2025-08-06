using System.Collections.Generic;
using System.Threading.Tasks;
using BreweryAPI.Models;

namespace BreweryAPI.Repositories.Interfaces
{
    public interface IBreweryRepository
    {
        Task<IEnumerable<Brewery>> GetAllBreweriesAsync(string? sortBy, double? userLat, double? userLng, int page, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<Brewery?> GetBreweryByIdAsync(string id);
        Task<IEnumerable<Brewery>> SearchAsync(string query);
        Task<IEnumerable<Brewery>> GetByCityAsync(string city);
        Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query);
    }
}
