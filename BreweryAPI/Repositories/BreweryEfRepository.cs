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

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync()
        {
            return await _context.Breweries.ToListAsync();
        }

        public async Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            return await _context.Breweries.FindAsync(id);
        }

        public async Task<IEnumerable<Brewery>> SearchAsync(string query)
        {
            return await _context.Breweries
                .Where(b => b.Name != null && b.Name.Contains(query)).ToListAsync();
        }

        public async Task<IEnumerable<Brewery>> GetByCityAsync(string city)
        {
            return await _context.Breweries
                .Where(b => b.City != null && b.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<IEnumerable<BreweryAutocomplete>> AutocompleteAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<BreweryAutocomplete>();

            return await _context.Breweries
                .Where(b => b.Name != null && b.Name.ToLower().Contains(query.ToLower()))
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