using System.Collections.Generic;
using System.Linq;
using BreweryAPI.Models;

namespace BreweryAPI.Helpers
{
    public static class SortingExtensions
    {
        public static IQueryable<Brewery> ApplySorting(this IQueryable<Brewery> query, string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                var s when s == SortOption.Name.ToString().ToLower() => query.OrderBy(b => b.Name ?? string.Empty),
                var s when s == SortOption.City.ToString().ToLower() => query.OrderBy(b => b.City ?? string.Empty),
                _ => query.OrderBy(b => b.Name ?? string.Empty)
            };
        }

        public static IEnumerable<Brewery> ApplySorting(this IEnumerable<Brewery> breweries, string? sortBy, double? lat, double? lng)
        {
            return sortBy?.ToLower() switch
            {
                var s when s == SortOption.Name.ToString().ToLower() => breweries.OrderBy(b => b.Name ?? string.Empty),
                var s when s == SortOption.City.ToString().ToLower() => breweries.OrderBy(b => b.City ?? string.Empty),
                var s when s == SortOption.Distance.ToString().ToLower() && lat.HasValue && lng.HasValue =>
                    breweries.OrderBy(b => GeoHelper.GetDistance(lat.Value, lng.Value, b.Latitude, b.Longitude)),
                _ => breweries.OrderBy(b => b.Name ?? string.Empty)
            };
        }
    }
}
