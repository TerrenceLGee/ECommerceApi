using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.Auth.Request;

public class LoginRequest
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}