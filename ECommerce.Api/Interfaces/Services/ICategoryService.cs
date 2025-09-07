using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Categories.Request;
using ECommerce.Api.Dtos.Categories.Response;
using ECommerce.Api.Dtos.Shared.Pagination;

namespace ECommerce.Api.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<Result<CategoryResponse>> DeleteCategoryAsync(int id);
    Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id);
    Task<Result<PagedList<CategoryResponse>>> GetAllCategoriesAsync(PaginationParams paginationParams);
}