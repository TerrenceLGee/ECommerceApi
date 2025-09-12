using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Auth.Errors;
using ECommerce.Presentation.Dtos.Auth.Request;
using ECommerce.Presentation.Dtos.Auth.Response;
using ECommerce.Presentation.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Presentation.Services;

public class LoginApiService : ILoginApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LoginApiService> _logger;
    public string? JwtToken { get; set; }
    
    public LoginApiService(
        HttpClient httpClient,
        ILogger<LoginApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<Result> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Login failed returned: {statusCode}.", response.StatusCode);
                return Result.Fail($"Login failed returned: {response.StatusCode}");
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse?.Token is null)
            {
                _logger.LogError("Login token invalid");
                return Result.Fail("Login token invalid");
            }

            JwtToken = authResponse.Token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred while attempting to login: {errorMessage}", ex.Message);
            return Result.Fail($"An unexpected error occurred while attempting to login: {ex.Message}");
        }
    }

    public async Task<Result<string>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var possibleErrorMessages = new StringBuilder();

                try
                {
                    var errors = JsonSerializer.Deserialize<List<IdentityError>>(errorContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errors is not null && errors.Any())
                    {
                        foreach (var error in errors)
                        {
                            _logger.LogError("Error: {errorDescription}", error.Description);
                            possibleErrorMessages.Append(error.Description);
                            possibleErrorMessages.Append(Environment.NewLine);
                        }

                        return Result<string>.Fail(possibleErrorMessages.ToString());
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogCritical("API Error: {errorContent}\n{errorMessage}", errorContent, ex.Message);
                    possibleErrorMessages.Append($"API Error: {errorContent}\n{ex.Message}");
                    return Result<string>.Fail(possibleErrorMessages.ToString());
                }
            }

            return Result<string>.Ok($"Registration successful for {request.Email}! You can now log in.");
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error while attempting to register new user: {errorMessage}", ex.Message);
            return Result<string>.Fail($"An unexpected error while attempting to register new user: {ex.Message}");
        }
    }

    public Result<ClaimsPrincipal> GetPrincipalFromToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(JwtToken))
            {
                return Result<ClaimsPrincipal>.Ok(new ClaimsPrincipal());
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            return Result<ClaimsPrincipal>.Ok(new ClaimsPrincipal(identity));
        }
        catch (SecurityTokenMalformedException ex)
        {
            _logger.LogCritical("Security Token Malformed: {errorMessage}", ex.Message);
            return Result<ClaimsPrincipal>.Fail($"Security Token Malformed: {ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("Argument cannot be null: {errorMessage}", ex.Message);
            return Result<ClaimsPrincipal>.Fail($"Argument cannot be null: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            _logger.LogCritical("Argument error: {errorMessage}", ex.Message);
            return Result<ClaimsPrincipal>.Fail($"Argument error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<ClaimsPrincipal>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}