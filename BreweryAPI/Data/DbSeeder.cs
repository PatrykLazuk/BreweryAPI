using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BreweryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BreweryAPI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(BreweryDbContext context)
        {
            if (await context.Breweries.AnyAsync()) return; // Database already seeded

            using var client = new HttpClient { BaseAddress = new Uri("https://api.openbrewerydb.org/v1/") };
            var breweries = await client.GetFromJsonAsync<List<Brewery>>("breweries?per_page=200");
            if (breweries != null)
            {
                context.Breweries.AddRange(breweries);
                await context.SaveChangesAsync();
            }
        }

    }
}