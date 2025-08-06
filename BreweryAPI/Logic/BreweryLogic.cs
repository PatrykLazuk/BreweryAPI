using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Helpers;
using BreweryAPI.Logic.Interfaces;
using BreweryAPI.Models;
using BreweryAPI.Repositories.Interfaces;

namespace BreweryAPI.Logic
{
    public class BreweryLogic : IBreweryLogic
    {
        private readonly IBreweryRepository _breweryRepository;

        public BreweryLogic(IBreweryRepository breweryRepository)
        {
            _breweryRepository = breweryRepository;
        }

        public async Task<PagedResult<Brewery>> GetAllBreweriesAsync(
            string? search, string? city, string? sortBy, double? userLat, double? userLng, int page, int pageSize)
        {
            IEnumerable<Brewery> data;
            int totalCount;

            if (!string.IsNullOrWhiteSpace(search))
            {
                data = await _breweryRepository.SearchAsync(search);
                totalCount = data.Count();
                data = ApplySorting(data, sortBy, userLat, userLng);
                data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(city))
            {
                data = await _breweryRepository.GetByCityAsync(city);
                totalCount = data.Count();
                data = ApplySorting(data, sortBy, userLat, userLng);
                data = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                // Tu paginacja i sortowanie już po stronie repozytorium!
                data = await _breweryRepository.GetAllBreweriesAsync(sortBy, userLat, userLng, page, pageSize);
                totalCount = await _breweryRepository.GetTotalCountAsync();
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PagedResult<Brewery>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = data
            };
        }

        public Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            return _breweryRepository.GetBreweryByIdAsync(id);
        }

        public Task<IEnumerable<Brewery>> SearchAsync(string query)
        {
            return _breweryRepository.SearchAsync(query);
        }

        public Task<IEnumerable<Brewery>> GetByCityAsync(string city)
        {
            return _breweryRepository.GetByCityAsync(city);
        }

        public async Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query)
        {
            return await _breweryRepository.AutocompleteAsync(query);
        }

        private IEnumerable<Brewery> ApplySorting(IEnumerable<Brewery> breweries, string? sortBy, double? lat, double? lng)
        {
            return sortBy?.ToLower() switch
            {
                "name" => breweries.OrderBy(b => b.Name),
                "city" => breweries.OrderBy(b => b.City),
                "distance" when lat.HasValue && lng.HasValue =>
                    breweries.OrderBy(b => GeoHelper.GetDistance(lat.Value, lng.Value, b.Latitude, b.Longitude)),
                _ => breweries.OrderBy(b => b.Name)
            };
        }
    }
}
