using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using BreweryAPI.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BreweryAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BreweriesController : ControllerBase
    {
        private readonly IBreweryLogic _breweryLogic;

        public BreweriesController(IBreweryLogic breweryLogic)
        {
            _breweryLogic = breweryLogic;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? city)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                var breweries = await _breweryLogic.SearchAsync(search);
                return Ok(breweries);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var breweries = await _breweryLogic.GetByCityAsync(city);
                return Ok(breweries);
            }

            var allBreweries = await _breweryLogic.GetAllBreweriesAsync();
            return Ok(allBreweries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBreweryById(string id)
        {
            var brewery = await _breweryLogic.GetBreweryByIdAsync(id);
            if (brewery == null)
            {
                return NotFound();
            }
            return Ok(brewery);
        }
    }
}