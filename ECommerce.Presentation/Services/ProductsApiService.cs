using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Products.Request;
using ECommerce.Presentation.Dtos.Products.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;
using ECommerce.Presentation.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce.Presentation.Services;

public class ProductsApiService : IProductsApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsApiService> _logger;

    public ProductsApiService(
        HttpClient httpClient,
        ILogger<ProductsApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<Result<PagedList<ProductResponse>?>> GetProductsAsync(PaginationParams paginationParams)
    {
        try
        {
            var queryString =
                $"api/products?pageNumber={paginationParams.PageNumber}&pageSize={paginationParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving products, returned: {statusCode}:", response.StatusCode);
                return Result<PagedList<ProductResponse>?>.Fail(
                    $"Error retrieving products, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<PagedList<ProductResponse>>(options);

            return Result<PagedList<ProductResponse>?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<PagedList<ProductResponse>?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<PagedList<ProductResponse>?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<PagedList<ProductResponse>?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<PagedList<ProductResponse>?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse?>> GetProductByIdAsync(int id)
    {
        try
        {
            var queryString = $"api/products/{id}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving product with Id {id}, returned: {statusCode}:", id,
                    response.StatusCode);
                return Result<ProductResponse?>.Fail(
                    $"Error retrieving product with Id {id}, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<ProductResponse>(options);

            return Result<ProductResponse?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<ProductResponse?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<ProductResponse?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<ProductResponse?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<ProductResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse?>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var queryString = "api/products";
            var response = await _httpClient.PostAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error creating product: {errorMessage}", errorContent);
                return Result<ProductResponse?>.Fail($"Error creating product {errorContent}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<ProductResponse>(options);

            return Result<ProductResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<ProductResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse?>> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var queryString = $"api/products/{id}";
            var response = await _httpClient.PutAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error updating product with Id {id}: {errorMessage}", id, errorContent);
                return Result<ProductResponse?>.Fail($"Error updating product with Id {id}: {errorContent}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<ProductResponse>(options);

            return Result<ProductResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<ProductResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse?>> DeleteProductAsync(int id)
    {
        try
        {
            var queryString = $"api/products/{id}";
            var response = await _httpClient.DeleteAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error deleing product with Id {id}: {errorMessage}", id, errorContent);
                return Result<ProductResponse?>.Fail($"Error deleting product with Id {id}: {errorContent}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<ProductResponse>(options);

            return Result<ProductResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<ProductResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}