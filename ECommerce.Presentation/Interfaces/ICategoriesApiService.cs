using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Categories.Request;
using ECommerce.Presentation.Dtos.Categories.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface ICategoriesApiService
{
    Task<Result<PagedList<CategoryResponse>?>> GetCategoriesAsync(PaginationParams paginationParams);
    Task<Result<CategoryResponse?>> GetCategoryByIdAsync(int id);
    Task<Result<CategoryResponse?>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<Result<CategoryResponse?>> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<Result<CategoryResponse?>> DeleteCategoryAsync(int id);
}