using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using BreweryAPI.Helpers;
using BreweryAPI.Logic.Interfaces;
using BreweryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BreweryAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BreweriesController : ControllerBase
    {
        private readonly IBreweryLogic _breweryLogic;

        public BreweriesController(IBreweryLogic breweryLogic)
        {
            _breweryLogic = breweryLogic ?? throw new ArgumentNullException(nameof(breweryLogic));
        }

        [HttpGet]
        [ApiVersionNeutral]
        [Authorize]
        public async Task<ActionResult<PagedResult<Brewery>>> Get(
            [FromQuery] string? search,
            [FromQuery] string? city,
            [FromQuery] string? sortBy,
            [FromQuery] double? userLat,
            [FromQuery] double? userLng,
            [FromQuery] int page = Constants.Pagination.DefaultPage,
            [FromQuery] int pageSize = Constants.Pagination.DefaultPageSize)
        {
            // Normalize pagination params
            var (normalizedPage, normalizedPageSize) = (page, pageSize).NormalizePagination();

            // Check if sort option is valid
            if (!sortBy.IsValidSortOption())
            {
                return BadRequest(new { error = Constants.ErrorMessages.InvalidSortByParameter });
            }

            // Distance sorting requires coordinates
            if (sortBy.RequiresCoordinates() && !(userLat, userLng).HasValidCoordinates())
            {
                return BadRequest(new { error = Constants.ErrorMessages.CoordinatesRequiredForDistance });
            }

            var result = await _breweryLogic.GetAllBreweriesAsync(search, city, sortBy, userLat, userLng, normalizedPage, normalizedPageSize);
            return Ok(result);
        }

        // v1 endpoint
        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBreweryByIdV1(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { error = Constants.ErrorMessages.BreweryIdRequired });

            var brewery = await _breweryLogic.GetBreweryByIdAsync(id);
            if (brewery == null)
            {
                return NotFound(new { error = Constants.ErrorMessages.BreweryNotFound });
            }
            return Ok(brewery);
        }

        // v2 endpoint with extra info
        [HttpGet("{id}")]
        [MapToApiVersion("2.0")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBreweryByIdV2(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { error = Constants.ErrorMessages.BreweryIdRequired });

            var brewery = await _breweryLogic.GetBreweryByIdAsync(id);

            if (brewery == null)
                return NotFound(new { error = Constants.ErrorMessages.BreweryNotFound });

            var result = new BreweryV2Dto
            {
                Brewery = brewery,
                Info = "This is a v2 response with additional information."
            };

            return Ok(result);
        }

        [HttpGet("autocomplete")]
        [ApiVersionNeutral]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BreweryAutocomplete>>> Autocomplete([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = Constants.ErrorMessages.QueryCannotBeEmpty });

            if (query.Length < Constants.Validation.MinQueryLength)
                return BadRequest(new { error = Constants.ErrorMessages.QueryTooShort });

            var results = await _breweryLogic.AutocompleteAsync(query);
            return Ok(results);
        }
    }
}