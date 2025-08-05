using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
    }
}