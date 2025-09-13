using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Categories.Request;
using ECommerce.Presentation.Dtos.Categories.Response;

namespace ECommerce.Presentation.Interfaces;

public interface ICategoriesApiService
{
    Task<Result<List<CategoryResponse>?>> GetCategoriesAsync(int pageNumber, int pageSize);
    Task<Result<List<CategoryResponse>?>> GetCategoriesForSummaryAsync();
    Task<Result<CategoryResponse?>> GetCategoryByIdAsync(int id);
    Task<Result<CategoryResponse?>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<Result<CategoryResponse?>> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<Result<CategoryResponse?>> DeleteCategoryAsync(int id);
    Task<Result<int>> GetCountOfCategoriesAsync();
}