using System.Security.Claims;
using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Auth.Request;

namespace ECommerce.Presentation.Interfaces;

public interface ILoginApiService
{
    string? JwtToken { get; set; }
    Task<Result> LoginAsync(string email, string password);
    Task<Result<string>> RegisterAsync(RegisterRequest request);
    Result<ClaimsPrincipal> GetPrincipalFromToken(string token);
}