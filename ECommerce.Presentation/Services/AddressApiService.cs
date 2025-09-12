using System.Net.Http.Json;
using System.Text.Json;
using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Request;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;
using ECommerce.Presentation.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Presentation.Services;

public class AddressApiService : IAddressApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AddressApiService> _logger;
    
    public AddressApiService(
        HttpClient httpClient,
        ILogger<AddressApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task<Result<AddressResponse?>> AddAddressAsync(CreateAddressRequest request)
    {
        try
        {
            var queryString = "api/addresses";
            var response = await _httpClient.PostAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error adding address, returned: {statusCode}", response.StatusCode);
                Result<AddressResponse?>.Fail($"Error adding address, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<AddressResponse>(options);

            return Result<AddressResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<AddressResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse?>> UpdateAddressAsync(int addressId, UpdateAddressRequest request)
    {
        try
        {
            var queryString = $"api/addresses/{addressId}";
            var response = await _httpClient.PutAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error updating address with Id {id}, returned: {statusCode}", addressId, response.StatusCode);
                return Result<AddressResponse?>.Fail(
                    $"Error updating address with Id {addressId}, returned {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<AddressResponse>(options);

            return Result<AddressResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<AddressResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse?>> DeleteAddressAsync(int addressId)
    {
        try
        {
            var queryString = $"api/addresses/{addressId}";
            var response = await _httpClient.DeleteAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error deleting address with Id {id}, returned: {statusCode}", addressId,
                    response.StatusCode);
                return Result<AddressResponse?>.Fail(
                    $"Error deleting address with Id {addressId}, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<AddressResponse>(options);

            return Result<AddressResponse?>.Ok(results);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<AddressResponse?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<AddressResponse?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<AddressResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<AddressResponse>?>> GetAllAddressesAsync(PaginationParams paginationParams)
    {
        try
        {
            var queryString = $"api/addresses?pageNumber={paginationParams.PageNumber}&pageSize={paginationParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving addresses, returned: {statusCode}", response.StatusCode);
                return Result<PagedList<AddressResponse>?>.Fail(
                    $"Error retrieving address, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<PagedList<AddressResponse>>(options);

            return Result<PagedList<AddressResponse>?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<PagedList<AddressResponse>?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<PagedList<AddressResponse>?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<PagedList<AddressResponse>?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<PagedList<AddressResponse>?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse?>> GetAddressByIdAsync(int addressId)
    {
        try
        {
            var queryString = $"api/addresses/{addressId}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving address with Id {id}, returned: {statusCode}", addressId,
                    response.StatusCode);
                return Result<AddressResponse?>.Fail(
                    $"Error retrieving address with Id {addressId}, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<AddressResponse>(options);

            return Result<AddressResponse?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<AddressResponse?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<AddressResponse?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<AddressResponse?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<AddressResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}