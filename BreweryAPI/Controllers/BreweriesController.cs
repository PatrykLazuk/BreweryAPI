using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using BreweryAPI.Logic.Interfaces;
using BreweryAPI.Models;
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
        public async Task<ActionResult<PagedResult<Brewery>>> Get(
            [FromQuery] string? search,
            [FromQuery] string? city,
            [FromQuery] string? sortBy,
            [FromQuery] double? userLat,
            [FromQuery] double? userLng,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 1 : (pageSize > 200 ? 200 : pageSize);

            var result = await _breweryLogic.GetAllBreweriesAsync(search, city, sortBy, userLat, userLng, page, pageSize);
            return Ok(result);
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

        [HttpGet("autocomplete")]
        public async Task<ActionResult<IEnumerable<BreweryAutocomplete>>> Autocomplete([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Query cannot be empty." });

            var results = await _breweryLogic.AutocompleteAsync(query);
            return Ok(results);
        }
    }
}