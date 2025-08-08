using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BreweryAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BreweryAPI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(BreweryDbContext context, ILogger? logger = null)
        {
            if (await context.Breweries.AnyAsync()) 
            {
                logger?.LogInformation("Database already seeded, skipping seeding process");
                return; // Skip if already seeded
            }

            try
            {
                logger?.LogInformation("Starting database seeding from OpenBreweryDB API");
                
                using var client = new HttpClient 
                { 
                    BaseAddress = new Uri(Constants.Api.BaseUrl),
                    Timeout = TimeSpan.FromSeconds(Constants.Api.TimeoutSeconds)
                };
                
                var breweries = await client.GetFromJsonAsync<List<Brewery>>($"breweries?per_page={Constants.Api.MaxBreweriesPerPage}");
                if (breweries != null && breweries.Any())
                {
                    context.Breweries.AddRange(breweries);
                    await context.SaveChangesAsync();
                    logger?.LogInformation("Successfully seeded {Count} breweries from OpenBreweryDB API", breweries.Count);
                }
                else
                {
                    logger?.LogWarning("No breweries received from OpenBreweryDB API");
                }
            }
            catch (HttpRequestException ex)
            {
                logger?.LogError(ex, "Failed to fetch breweries from OpenBreweryDB API during seeding");
                // Continue startup even if seeding fails
            }
            catch (TaskCanceledException ex)
            {
                logger?.LogError(ex, "Timeout while fetching breweries from OpenBreweryDB API during seeding");
                // Continue startup even if seeding fails
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unexpected error during database seeding");
                // Continue startup even if seeding fails
            }
        }
    }
}