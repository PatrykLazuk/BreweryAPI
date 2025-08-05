using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BreweryAPI.Models;
using BreweryAPI.Repositories.Interfaces;

namespace BreweryAPI.Repositories
{
    public class BreweryApiRepository : IBreweryRepository
    {
        private readonly HttpClient _httpClient;

        public BreweryApiRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync()
        {
            var list = await _httpClient.GetFromJsonAsync<List<Brewery>>("breweries?per_page=200");
            return list ?? new List<Brewery>();
        }

        public async Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Brewery>($"breweries/{Uri.EscapeDataString(id)}");
        }
    }
}