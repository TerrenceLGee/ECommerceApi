using System.Net.Http.Json;
using System.Text.Json;
using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Auth.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;
using ECommerce.Presentation.Interfaces;
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
    
    public async Task<Result<PagedList<UserResponse>>> GetAllUsersAsync(PaginationParams paginationParams)
    {
        try
        {
            var queryString = $"api/users/?pageNumber={paginationParams.PageNumber}&{paginationParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving all users, returned: {statusCode}", response.StatusCode);
                return Result<PagedList<UserResponse>>.Fail(
                    $"Error retrieving all users, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<PagedList<UserResponse>>(options);

            if (results is null)
            {
                _logger.LogError("Error reading users from Json");
                return Result<PagedList<UserResponse>>.Fail("Error reading users from Json");
            }
            
            return Result<PagedList<UserResponse>>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<PagedList<UserResponse>>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<PagedList<UserResponse>>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<PagedList<UserResponse>>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<PagedList<UserResponse>>.Fail($"An unexpected error occurred: {ex.Message}");
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

    public async Task<Result<PagedList<AddressResponse>>> GetUserAddressesByIdAsync(string userId, PaginationParams paginationParams)
    {
        try
        {
            var queryString = "api/users/{userId}/addresses";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving addresses for user with Id {id}, returned: {statusCode}", userId,
                    response.StatusCode);
                return Result<PagedList<AddressResponse>>.Fail(
                    $"Error retrieving address for user Id {userId}, returned {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<PagedList<AddressResponse>>(options);

            if (results is null)
            {
                _logger.LogError("Error reading addresses from Json");
                return Result<PagedList<AddressResponse>>.Fail("Error reading address from Json");
            }

            return Result<PagedList<AddressResponse>>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<PagedList<AddressResponse>>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<PagedList<AddressResponse>>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<PagedList<AddressResponse>>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<PagedList<AddressResponse>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}