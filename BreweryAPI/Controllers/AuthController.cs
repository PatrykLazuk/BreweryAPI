using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using BreweryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BreweryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("token")]
        public IActionResult GetToken([FromBody] BreweryLoginRequest request)
        {
            if (request == null)
                return BadRequest(new { error = Constants.ErrorMessages.RequestBodyRequired });

            // Check model validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { error = Constants.ErrorMessages.ValidationFailed, details = errors });
            }

            var defaultUser = _configuration.GetSection(Constants.Configuration.DefaultUserSection);
            var expectedUsername = defaultUser[Constants.Configuration.UsernameKey];
            var expectedPassword = defaultUser[Constants.Configuration.PasswordKey];

            if (string.IsNullOrEmpty(expectedUsername) || string.IsNullOrEmpty(expectedPassword))
                return StatusCode(500, new { error = Constants.ErrorMessages.ServerConfigurationError });

            if (request.Username != expectedUsername || request.Password != expectedPassword)
                return Unauthorized(new { error = Constants.ErrorMessages.InvalidCredentials });

            var jwtSettings = _configuration.GetSection(Constants.Configuration.JwtSettingsSection);
            var secretKey = jwtSettings[Constants.Configuration.SecretKey];
            var issuer = jwtSettings[Constants.Configuration.IssuerKey];
            var audience = jwtSettings[Constants.Configuration.AudienceKey];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                return StatusCode(500, new { error = Constants.ErrorMessages.JwtConfigurationError });

            try
            {
                var key = Convert.FromBase64String(secretKey);
                var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, "User")
                };

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(Constants.Security.JwtExpirationHours),
                    signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { access_token = tokenString });
            }
            catch (FormatException)
            {
                return StatusCode(500, new { error = Constants.ErrorMessages.InvalidJwtConfiguration });
            }
        }
    }
}