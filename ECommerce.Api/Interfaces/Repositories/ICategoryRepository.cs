using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Models;

namespace ECommerce.Api.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task<bool> ExistsAsync(int id);
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<PagedList<Category>> GetCategoriesAsync(PaginationParams paginationParams);
    Task<int> GetCountOfCategoriesAsync();
}