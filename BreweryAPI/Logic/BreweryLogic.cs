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

        public async Task<IEnumerable<Brewery>> GetAllBreweriesAsync(string? search, string? city, string? sortBy, double? userLat, double? userLng, int page, int pageSize)
        {
            IEnumerable<Brewery> data;
            if (!string.IsNullOrWhiteSpace(search))
                data = await _breweryRepository.SearchAsync(search);
            else if (!string.IsNullOrWhiteSpace(city))
                data = await _breweryRepository.GetByCityAsync(city);
            else
                data = await _breweryRepository.GetAllBreweriesAsync();

            if (!string.IsNullOrWhiteSpace(sortBy))
                data = Sort(data, sortBy, userLat, userLng);

            data = data.Skip((page - 1) * pageSize).Take(pageSize);

            return data;
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

        private IEnumerable<Brewery> Sort(IEnumerable<Brewery> breweries, string sortBy, double? lat, double? lng)
        {
            return sortBy.ToLower() switch
            {
                "name" => breweries.OrderBy(b => b.Name),
                "city" => breweries.OrderBy(b => b.City),
                "distance" when lat.HasValue && lng.HasValue =>
                    breweries.OrderBy(b => GetDistance(lat.Value, lng.Value, b.Latitude, b.Longitude)),
                _ => breweries
            };
        }

        private double GetDistance(double lat1, double lon1, double? lat2, double? lon2)
        {
            if (!lat2.HasValue || !lon2.HasValue)
                return double.MaxValue; // Return a large value if user coordinates are not provided

            const double R = 6371; // metres
            // Convert degrees to radians
            var dLat = ToRad(lat2.Value - lat1);
            var dLon = ToRad(lon2.Value - lon1);
            // Haversine formula
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2.Value)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Distance in metres
            return R * c;

        }

        private double ToRad(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}