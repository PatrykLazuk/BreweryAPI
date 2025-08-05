using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BreweryAPI.Models;
using BreweryAPI.Repositories.Interfaces;

namespace BreweryAPI.Repositories
{
    public class BreweryApiRepository : IBreweryRepository
    {
        public Task<IEnumerable<Brewery>> GetAllBreweriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}