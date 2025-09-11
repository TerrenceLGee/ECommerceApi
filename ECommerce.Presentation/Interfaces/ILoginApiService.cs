using System.Security.Claims;
using ECommerce.Presentation.Dtos.Auth.Request;

namespace ECommerce.Presentation.Interfaces;

public interface ILoginApiService
{
    string? JwtToken { get; set; }
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterRequest request);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}