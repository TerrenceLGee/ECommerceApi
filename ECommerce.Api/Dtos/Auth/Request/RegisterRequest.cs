using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.Auth.Request;

public class RegisterRequest
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    [Required]
    public string StreetNumber { get; set; } = string.Empty;
    [Required]
    public string StreetName { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string State { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public string ZipCode { get; set; } = string.Empty;
    public string? Description { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

}