using System.Net.Http.Json;
using System.Text.Json;
using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Auth.Response;
using ECommerce.Presentation.Interfaces.Api;
using Microsoft.Extensions.Logging;

namespace ECommerce.Presentation.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserApiService> _logger;

    public UserApiService(
        HttpClient httpClient,
        ILogger<UserApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<Result<List<UserResponse>>> GetAllUsersAsync(int pageNumber, int pageSize)
    {
        try
        {
            var queryString = $"api/users?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving all users, returned: {statusCode}", response.StatusCode);
                return Result<List<UserResponse>>.Fail(
                    $"Error retrieving all users, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<List<UserResponse>>(options);

            if (results is null)
            {
                _logger.LogError("Error reading users from Json");
                return Result<List<UserResponse>>.Fail("Error reading users from Json");
            }
            
            return Result<List<UserResponse>>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<List<UserResponse>>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<List<UserResponse>>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<List<UserResponse>>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<List<UserResponse>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<UserResponse?>> GetUserByIdAsync(string userId)
    {
        try
        {
            var queryString = $"api/users/{userId}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving user with Id {id}, returned {statusCode}", userId,
                    response.StatusCode);
                return Result<UserResponse?>.Fail(
                    $"Error retrieving user with Id {userId}, returned {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<UserResponse>(options);

            return Result<UserResponse?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<UserResponse?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<UserResponse?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<UserResponse?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<UserResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
        
    }

    public async Task<Result<List<AddressResponse>>> GetUserAddressesByIdAsync(string userId, int pageNumber, int pageSize)
    {
        try
        {
            var queryString = $"api/users/{userId}/addresses?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving addresses for user with Id {id}, returned: {statusCode}", userId,
                    response.StatusCode);
                return Result<List<AddressResponse>>.Fail(
                    $"Error retrieving address for user Id {userId}, returned {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<List<AddressResponse>>(options);

            if (results is null)
            {
                _logger.LogError("Error reading addresses from Json");
                return Result<List<AddressResponse>>.Fail("Error reading address from Json");
            }

            return Result<List<AddressResponse>>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<List<AddressResponse>>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<List<AddressResponse>>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<List<AddressResponse>>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<List<AddressResponse>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetCountOfUsersAsync()
    {
        try
        {
            var queryString = "api/users/count";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting count of users, returned: {statusCode}", response.StatusCode);
                return Result<int>.Fail($"Error getting count of users, returned {response.StatusCode}");
            }

            var results = await response.Content.ReadAsStringAsync();

            if (int.TryParse(results, out var count))
            {
                return Result<int>.Ok(count);
            }
            else
            {
                _logger.LogError("Error retrieving count of users");
                return Result<int>.Fail("Error retrieving count of users");
            }
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<int>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<int>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<int>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<int>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}