using System.Security.Claims;
using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Auth.Request;

namespace ECommerce.Presentation.Interfaces.Api;

public interface ILoginApiService
{
    string? JwtToken { get; set; }
    Task<Result> LoginAsync(LoginRequest request);
    Task<Result<string>> RegisterAsync(RegisterRequest request);
    Result<ClaimsPrincipal> GetPrincipalFromToken(string token);
}