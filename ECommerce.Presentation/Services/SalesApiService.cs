using System.Net.Http.Json;
using System.Text.Json;
using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Sales.Request;
using ECommerce.Presentation.Dtos.Sales.Response;
using ECommerce.Presentation.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Presentation.Services;

public class SalesApiService : ISalesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SalesApiService> _logger;

    public SalesApiService(
        HttpClient httpClient,
        ILogger<SalesApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<Result<SaleResponse?>> CreateSaleAsync(CreateSaleRequest saleRequest)
    {
        try
        {
            var queryString = "api/sales";
            var response = await _httpClient.PostAsJsonAsync(queryString, saleRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error creating sale: {errorMessage} ", errorContent);
                return Result<SaleResponse?>.Fail($"Error creating sale: {errorContent}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<SaleResponse>(options);

            return Result<SaleResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<SaleResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
        
    }

    public async Task<Result<string?>> UpdateSaleAsync(int id, UpdateSaleStatusRequest updateRequest)
    {
        try
        {
            var queryString = $"api/sales/{id}/status";
            var response = await _httpClient.PutAsJsonAsync(queryString, updateRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error updating sale status: {errorMessage}", errorContent);
                return Result<string?>.Fail($"Error updating sale status: {errorContent}");
            }

            var successMessage =  $"Sale with Id {id} updated successfully.\nHttp status code returned: {response.StatusCode}";

            return Result<string?>.Ok(successMessage);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<string?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<string?>> RefundSaleAsync(int id)
    {
        try
        {
            var queryString = $"api/sales/{id}/refund";
            var response = await _httpClient.PutAsync(queryString, null);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error refunding sale: {errorMessage}", errorContent);
                return Result<string?>.Fail($"Error refunding sale: {errorContent}");
            }

            var successMessage = $"Sale with Id {id} refunded successfully.\nHttp status code returned: {response.StatusCode}";
            return Result<string?>.Ok(successMessage);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<string?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<string?>> CancelSaleAsync(int id)
    {
        try
        {
            var queryString = $"api/sales/{id}/cancel";
            var response = await _httpClient.PostAsync(queryString, null);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error canceling sale: {errorMessage}", errorContent);
                return Result<string?>.Fail($"Error canceling sale: {errorContent}");
            }

            var successMessage = $"Sale with Id {id} canceled successfully.\nHttp status code returned: {response.StatusCode}";
            
            return Result<string?>.Ok(successMessage);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<string?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<string?>> UserCancelSaleAsync(int id)
    {
        try
        {
            var queryString = $"api/sales/me/sales/cancel/{id}";
            var response = await _httpClient.PostAsync(queryString, null);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error canceling sale: {errorMessage}", errorContent);
                return Result<string?>.Fail($"Error canceling sale: {errorContent}");
            }

            var successMessage = $"Sale with Id {id} canceled successfully.";
            return Result<string?>.Ok(successMessage);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<string?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<string?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<List<SaleResponse>?>> GetAllSalesAsync(int pageNumber, int pageSize)
    {
        try
        {
            var queryString =
                $"api/sales?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving sales, returned: {statusCode}:", response.StatusCode);
                return Result<List<SaleResponse>?>.Fail(
                    $"Error retrieving sales, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<List<SaleResponse>>(options);

            return Result<List<SaleResponse>?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<List<SaleResponse>?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<List<SaleResponse>?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<List<SaleResponse>?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<List<SaleResponse>?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<List<SaleResponse>?>> GetAllSalesForUserAsync(int pageNumber, int pageSize)
    {
        try
        {
            var queryString =
                $"api/sales/me/sales?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving sales, returned: {statusCode}:", response.StatusCode);
                return Result<List<SaleResponse>?>.Fail($"Error retrieving sales, returned {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<List<SaleResponse>>(options);

            return Result<List<SaleResponse>?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<List<SaleResponse>?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<List<SaleResponse>?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<List<SaleResponse>?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<List<SaleResponse>?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<SaleResponse?>> GetSaleForUserByIdAsync(int saleId)
    {
        try
        {
            var queryString = $"api/sales/me/sales/{saleId}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving sale with Id {id}, returned: {statusCode}", saleId, response.StatusCode);
                return Result<SaleResponse?>.Fail(
                    $"Error retrieving sale with Id {saleId}, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<SaleResponse>(options);

            return Result<SaleResponse?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<SaleResponse?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<SaleResponse?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<SaleResponse?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<SaleResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<SaleResponse?>> GetSaleByIdAsync(int saleId)
    {
        try
        {
            var queryString = $"api/sales/{saleId}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving sale with Id {id}, returned: {statusCode}", saleId, response.StatusCode);
                return Result<SaleResponse?>.Fail(
                    $"Error retrieving sale with Id {saleId}, returned {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<SaleResponse>(options);

            return Result<SaleResponse?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<SaleResponse?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<SaleResponse?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<SaleResponse?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<SaleResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetCountOfSalesAsync()
    {
        try
        {
            var queryString = "api/sales/count";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting count of sales, returned: {statusCode}", response.StatusCode);
                return Result<int>.Fail($"Error getting count of sales, returned: {response.StatusCode}");
            }

            var results = await response.Content.ReadAsStringAsync();

            if (int.TryParse(results, out var count))
            {
                return Result<int>.Ok(count);
            }
            else
            {
                _logger.LogError("Error retrieving count of sales");
                return Result<int>.Fail("Error retrieving count of sales");
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

    public async Task<Result<int>> GetCountOfSalesForUserAsync()
    {
        try
        {
            var queryString = "api/sales/me/sales/count";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error getting count of sales, returned: {statusCode}", response.StatusCode);
                return Result<int>.Fail($"Error getting count of sales, returned: {response.StatusCode}");
            }

            var results = await response.Content.ReadAsStringAsync();

            if (int.TryParse(results, out var count))
            {
                return Result<int>.Ok(count);
            }
            else
            {
                _logger.LogError("Error retrieving count of sales");
                return Result<int>.Fail("Error retrieving count of sales");
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