using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BreweryAPI.Data
{
    public class BreweryDbContext : DbContext
    {
        public BreweryDbContext(DbContextOptions<BreweryDbContext> options)
            : base(options) { }

        public DbSet<Brewery> Breweries { get; set; }
    }
}