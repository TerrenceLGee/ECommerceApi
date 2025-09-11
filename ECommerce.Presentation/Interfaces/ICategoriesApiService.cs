using ECommerce.Presentation.Dtos.Categories.Request;
using ECommerce.Presentation.Dtos.Categories.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface ICategoriesApiService
{
    Task<PagedList<CategoryResponse>?> GetCategoriesAsync(PaginationParams paginationParams);
    Task<CategoryResponse?> GetCategoryByIdAsync(int id);
    Task<CategoryResponse?> CreateCategoryAsync(CreateCategoryRequest request);
    Task<CategoryResponse?> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<CategoryResponse?> DeleteCategoryAsync(int id);
}