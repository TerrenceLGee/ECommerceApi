using System.ComponentModel.DataAnnotations;

namespace ECommerce.Presentation.Dtos.Auth.Request;

public class RegisterRequest
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required] public DateOnly BirthDate { get; set; }
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required] public string StreetNumber { get; set; } = string.Empty;
    [Required] public string StreetName { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
    [Required] public string State { get; set; } = string.Empty;
    [Required] public string Country { get; set; } = string.Empty;
    [Required] public string AddressDescription { get; set; } = string.Empty;
}