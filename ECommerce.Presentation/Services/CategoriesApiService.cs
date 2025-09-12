using System.Net.Http.Json;
using System.Text.Json;
using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Categories.Request;
using ECommerce.Presentation.Dtos.Categories.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;
using ECommerce.Presentation.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Presentation.Services;

public class CategoriesApiService : ICategoriesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CategoriesApiService> _logger;

    public CategoriesApiService(
        HttpClient httpClient,
        ILogger<CategoriesApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<Result<PagedList<CategoryResponse>?>> GetCategoriesAsync(PaginationParams paginationParams)
    {
        try
        {
            var queryString =
                $"api/categories?pageNumber={paginationParams.PageNumber}&pageSize={paginationParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving categories returned: {statusCode}:", response.StatusCode);
                return Result<PagedList<CategoryResponse>?>.Fail(
                    $"Error retrieving categories returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<PagedList<CategoryResponse>>(options);
            
            return Result<PagedList<CategoryResponse>?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<PagedList<CategoryResponse>?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<PagedList<CategoryResponse>?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<PagedList<CategoryResponse>?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<PagedList<CategoryResponse>?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse?>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var queryString = $"api/categories/{id}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving category with Id {id}, returned: {statusCode}:", id, response.StatusCode);
                return Result<CategoryResponse?>.Fail(
                    $"Error retrieving category with Id {id}, returned: {response.StatusCode}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<CategoryResponse>(options);

            return Result<CategoryResponse?>.Ok(results);
        }
        catch (UriFormatException ex)
        {
            _logger.LogCritical("Uri Format Error: {errorMessage}", ex.Message);
            return Result<CategoryResponse?>.Fail($"Uri Format Error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogCritical("Http Request Error: {errorMessage}", ex.Message);
            return Result<CategoryResponse?>.Fail($"Http Request Error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical("Invalid Operation Error: {errorMessage}", ex.Message);
            return Result<CategoryResponse?>.Fail($"Invalid Operation Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<CategoryResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse?>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        try
        {
            var queryString = "api/categories";

            var response = await _httpClient.PostAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error creating category: {errorMessage}", errorContent);
                return Result<CategoryResponse?>.Fail($"Error creating category: {errorContent}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<CategoryResponse>(options);

            return Result<CategoryResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<CategoryResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse?>> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        try
        {
            var queryString = $"api/categories/{id}";

            var response = await _httpClient.PutAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error updating category with Id {id}: {errorMessage}", id, errorContent);
                return Result<CategoryResponse?>.Fail($"Error updating category with Id {id}: {errorContent}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<CategoryResponse>(options);

            return Result<CategoryResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<CategoryResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse?>> DeleteCategoryAsync(int id)
    {
        try
        {
            var queryString = $"api/categories/{id}";

            var response = await _httpClient.DeleteAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error deleting category with Id {id}: {errorMessage} ", id, errorContent);
                return Result<CategoryResponse?>.Fail($"Error deleting category with Id {id}: {errorContent}");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = await response.Content.ReadFromJsonAsync<CategoryResponse>(options);

            return Result<CategoryResponse?>.Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}: ", ex.Message);
            return Result<CategoryResponse?>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}