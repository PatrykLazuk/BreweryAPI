using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Data;
using BreweryAPI.Models;
using BreweryAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BreweryAPI.Repositories
{
    public class BreweryEfRepository : IBreweryRepository
    {
        private readonly BreweryDbContext _context;

        public BreweryEfRepository(BreweryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync(string? sortBy, double? userLat, double? userLng, int page, int pageSize)
        {
            IQueryable<Brewery> query = _context.Breweries;

            // Sortowanie w bazie (po "distance" sortowanie już w logice)
            query = sortBy?.ToLower() switch
            {
                "name" => query.OrderBy(b => b.Name),
                "city" => query.OrderBy(b => b.City),
                _ => query.OrderBy(b => b.Name)
            };

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Breweries.CountAsync();
        }

        public async Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            return await _context.Breweries.FindAsync(id);
        }

        public async Task<IEnumerable<Brewery>> SearchAsync(string query)
        {
            return await _context.Breweries
                .Where(b => b.Name != null && b.Name.Contains(query))
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Brewery>> GetByCityAsync(string city)
        {
            return await _context.Breweries
                .Where(b => b.City != null && b.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<BreweryAutocomplete>();

            return await _context.Breweries
                .Where(b => b.Name != null && b.Name.ToLower().Contains(query.ToLower()))
                .OrderBy(b => b.Name)
                .Select(b => new BreweryAutocomplete
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .Take(15)
                .ToListAsync();
        }
    }
}
