using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Data;
using BreweryAPI.Helpers;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync(string? sortBy, double? userLat, double? userLng, int page, int pageSize)
        {
            IQueryable<Brewery> query = _context.Breweries;

            // Apply sorting
            query = query.ApplySorting(sortBy);

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
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return await _context.Breweries.FindAsync(id);
        }

        public async Task<IEnumerable<Brewery>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<Brewery>();

            return await _context.Breweries
                .Where(b => b.Name != null && b.Name.Contains(query))
                .OrderBy(b => b.Name ?? string.Empty)
                .ToListAsync();
        }

        public async Task<IEnumerable<Brewery>> GetByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return Enumerable.Empty<Brewery>();

            return await _context.Breweries
                .Where(b => b.City != null && b.City.ToLower() == city.ToLower())
                .OrderBy(b => b.Name ?? string.Empty)
                .ToListAsync();
        }

        public async Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<BreweryAutocomplete>();

            return await _context.Breweries
                .Where(b => b.Name != null && b.Name.ToLower().Contains(query.ToLower()))
                .OrderBy(b => b.Name ?? string.Empty)
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
