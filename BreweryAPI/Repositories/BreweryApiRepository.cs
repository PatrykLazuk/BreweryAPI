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

namespace BreweryAPI.Repositories
{
    public class BreweryApiRepository : IBreweryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private static readonly string AllBreweriesCacheKey = "all-breweries";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public BreweryApiRepository(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync(string? sortBy, double? userLat, double? userLng, int page, int pageSize)
        {
            var list = await GetAllBreweriesAsync();
            IEnumerable<Brewery> sorted = sortBy?.ToLower() switch
            {
                "name" => list.OrderBy(b => b.Name),
                "city" => list.OrderBy(b => b.City),
                "distance" when userLat.HasValue && userLng.HasValue =>
                    list.OrderBy(b => GeoHelper.GetDistance(userLat.Value, userLng.Value, b.Latitude, b.Longitude)),
                _ => list.OrderBy(b => b.Name)
            };

            return sorted.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var list = await GetAllBreweriesAsync();
            return list.Count();
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync()
        {
            if (_cache.TryGetValue(AllBreweriesCacheKey, out List<Brewery>? cachedBreweries) && cachedBreweries != null)
            {
                return cachedBreweries;
            }

            var list = await _httpClient.GetFromJsonAsync<List<Brewery>>("breweries?per_page=200");
            _cache.Set(AllBreweriesCacheKey, list, CacheDuration);
            return list ?? new List<Brewery>();
        }

        public async Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Brewery>($"breweries/{Uri.EscapeDataString(id)}");
        }

        public async Task<IEnumerable<Brewery>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAllBreweriesAsync();
            }

            var result = await _httpClient.GetFromJsonAsync<List<Brewery>>($"breweries/search?query={Uri.EscapeDataString(query)}");
            return result ?? new List<Brewery>();
        }

        public async Task<IEnumerable<Brewery>> GetByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return await GetAllBreweriesAsync();
            }
            var result = await _httpClient.GetFromJsonAsync<List<Brewery>>($"breweries?by_city={Uri.EscapeDataString(city)}");
            return result ?? new List<Brewery>();
        }

        public async Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<BreweryAutocomplete>();
            }

            var result = await _httpClient.GetFromJsonAsync<List<BreweryAutocomplete>>(
                $"breweries/autocomplete?query={Uri.EscapeDataString(query)}");
            return result ?? new List<BreweryAutocomplete>();
        }
    }
}
