using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BreweryAPI.Helpers;
using BreweryAPI.Models;
using BreweryAPI.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BreweryAPI.Repositories
{
    public class BreweryApiRepository : IBreweryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BreweryApiRepository> _logger;

        public BreweryApiRepository(HttpClient httpClient, IMemoryCache cache, ILogger<BreweryApiRepository> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync(string? sortBy, double? userLat, double? userLng, int page, int pageSize)
        {
            var list = await GetAllBreweriesAsync();
            
            // Sort the results
            var sorted = list.ApplySorting(sortBy, userLat, userLng);

            return sorted.ApplyPagination(page, pageSize).ToList();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var list = await GetAllBreweriesAsync();
            return list.Count();
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync()
        {
            if (_cache.TryGetValue(Constants.Cache.AllBreweriesKey, out List<Brewery>? cachedBreweries) && cachedBreweries != null)
            {
                return cachedBreweries;
            }

            try
            {
                var list = await _httpClient.GetFromJsonAsync<List<Brewery>>($"breweries?per_page={Constants.Api.MaxBreweriesPerPage}");
                var breweries = list ?? new List<Brewery>();
                _cache.Set(Constants.Cache.AllBreweriesKey, breweries, TimeSpan.FromMinutes(Constants.Cache.AllBreweriesCacheMinutes));
                return breweries;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch breweries from external API");
                return new List<Brewery>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while fetching breweries from external API");
                return new List<Brewery>();
            }
        }

        public async Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            // Check cache first
            var cacheKey = $"{Constants.Cache.BreweryPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out Brewery? cachedBrewery) && cachedBrewery != null)
            {
                return cachedBrewery;
            }

            try
            {
                var brewery = await _httpClient.GetFromJsonAsync<Brewery>($"breweries/{Uri.EscapeDataString(id)}");
                if (brewery != null)
                {
                    // Cache for 5 minutes
                    _cache.Set(cacheKey, brewery, TimeSpan.FromMinutes(Constants.Cache.IndividualBreweryCacheMinutes));
                }
                return brewery;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch brewery with ID {BreweryId} from external API", id);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while fetching brewery with ID {BreweryId} from external API", id);
                return null;
            }
        }

        public async Task<IEnumerable<Brewery>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAllBreweriesAsync();
            }

            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Brewery>>($"breweries/search?query={Uri.EscapeDataString(query)}");
                return result ?? new List<Brewery>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to search breweries with query '{Query}' from external API", query);
                return new List<Brewery>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while searching breweries with query '{Query}' from external API", query);
                return new List<Brewery>();
            }
        }

        public async Task<IEnumerable<Brewery>> GetByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return await GetAllBreweriesAsync();
            }

            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Brewery>>($"breweries?by_city={Uri.EscapeDataString(city)}");
                return result ?? new List<Brewery>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch breweries for city '{City}' from external API", city);
                return new List<Brewery>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while fetching breweries for city '{City}' from external API", city);
                return new List<Brewery>();
            }
        }

        public async Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<BreweryAutocomplete>();
            }

            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<BreweryAutocomplete>>(
                    $"breweries/autocomplete?query={Uri.EscapeDataString(query)}");
                return result ?? new List<BreweryAutocomplete>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to autocomplete breweries with query '{Query}' from external API", query);
                return new List<BreweryAutocomplete>();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while autocompleting breweries with query '{Query}' from external API", query);
                return new List<BreweryAutocomplete>();
            }
        }
    }
}
