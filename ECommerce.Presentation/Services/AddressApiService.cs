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

    public async Task<Result<List<AddressResponse>?>> GetAllAddressesAsync(int pageNumber, int pageSize)
    {
        try
        {
            var queryString = $"api/addresses?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving addresses, returned: {statusCode}", response.StatusCode);
                return Result<List<AddressResponse>?>.Fail(
                    $"Error retrieving address, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<List<AddressResponse>>(options);

            return Result<List<AddressResponse>?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<List<AddressResponse>?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<List<AddressResponse>?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<List<AddressResponse>?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<List<AddressResponse>?>.Fail($"An unexpected error occurred: {ex.Message}");
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

    public async Task<Result<int>> GetCountOfAddressesAsync()
    {
        try
        {
            var queryString = "api/addresses/count";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting count of addresses, returned: {statusCode}", response.StatusCode);
                return Result<int>.Fail($"Error getting count of addresses, returned: {response.StatusCode}");
            }

            var results = await response.Content.ReadAsStringAsync();

            if (int.TryParse(results, out var count))
            {
                return Result<int>.Ok(count);
            }
            else
            {
                _logger.LogError("Error retrieving count of addresses");
                return Result<int>.Fail("Error retrieving count of addresses");
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