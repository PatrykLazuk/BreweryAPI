using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreweryAPI.Logic.Interfaces;
using BreweryAPI.Models;
using BreweryAPI.Repositories.Interfaces;

namespace BreweryAPI.Logic
{
    public class BreweryLogic : IBreweryLogic
    {
        private readonly IBreweryRepository _breweryRepository;

        public BreweryLogic(IBreweryRepository breweryRepository)
        {
            _breweryRepository = breweryRepository;
        }

        public Task<IEnumerable<Brewery>> GetAllBreweriesAsync()
        {
            return _breweryRepository.GetAllBreweriesAsync();
        }

        public Task<Brewery?> GetBreweryByIdAsync(string id)
        {
            return _breweryRepository.GetBreweryByIdAsync(id);
        }
        public Task<IEnumerable<Brewery>> SearchAsync(string query)
        {
            return _breweryRepository.SearchAsync(query);
        }
        public Task<IEnumerable<Brewery>> GetByCityAsync(string city)
        {
            return _breweryRepository.GetByCityAsync(city);
        }
    }
}