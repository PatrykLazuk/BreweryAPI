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
            _breweryRepository = breweryRepository ?? throw new ArgumentNullException(nameof(breweryRepository));
        }

        public async Task<PagedResult<Brewery>> GetAllBreweriesAsync(
            string? search, string? city, string? sortBy, double? userLat, double? userLng, int page, int pageSize)
        {
            IEnumerable<Brewery> data;
            int totalCount;

            // Filter by search term or city
            if (!string.IsNullOrWhiteSpace(search))
            {
                data = await _breweryRepository.SearchAsync(search);
                totalCount = data.Count();
                data = data.ApplySorting(sortBy, userLat, userLng);
            }
            else if (!string.IsNullOrWhiteSpace(city))
            {
                data = await _breweryRepository.GetByCityAsync(city);
                totalCount = data.Count();
                data = data.ApplySorting(sortBy, userLat, userLng);
            }
            else
            {
                // Special case: distance sorting needs all breweries
                if (sortBy.RequiresCoordinates() && (userLat, userLng).HasValidCoordinates())
                {
                    var allData = await _breweryRepository.GetAllBreweriesAsync(null, null, null, Constants.Pagination.DefaultPage, int.MaxValue);
                    data = allData.ApplySorting(sortBy, userLat, userLng);
                    totalCount = data.Count();
                }
                else
                {
                    // Standard pagination from repository
                    data = await _breweryRepository.GetAllBreweriesAsync(sortBy, userLat, userLng, page, pageSize);
                    totalCount = await _breweryRepository.GetTotalCountAsync();
                    
                    // Repository already applied pagination
                    return data.CreatePagedResult(totalCount, page, pageSize);
                }
            }

            // Apply pagination to filtered results
            var paginatedData = data.ApplyPagination(page, pageSize).ToList();
            return paginatedData.CreatePagedResult(totalCount, page, pageSize);
        }

        public Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Task.FromResult<Brewery?>(null);
                
            return _breweryRepository.GetBreweryByIdAsync(id);
        }

        public Task<IEnumerable<Brewery>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Task.FromResult<IEnumerable<Brewery>>(Enumerable.Empty<Brewery>());
                
            return _breweryRepository.SearchAsync(query);
        }

        public Task<IEnumerable<Brewery>> GetByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return Task.FromResult<IEnumerable<Brewery>>(Enumerable.Empty<Brewery>());
                
            return _breweryRepository.GetByCityAsync(city);
        }

        public async Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<BreweryAutocomplete>();
                
            return await _breweryRepository.AutocompleteAsync(query);
        }
    }
}
