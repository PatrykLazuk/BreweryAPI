using System.ComponentModel.DataAnnotations;

namespace BreweryAPI.Models
{
    public class BreweryLoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(Constants.Validation.MaxUsernameLength, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(Constants.Validation.MaxPasswordLength, MinimumLength = 1, ErrorMessage = "Password must be between 1 and 100 characters")]
        public string Password { get; set; } = string.Empty;
    }
}