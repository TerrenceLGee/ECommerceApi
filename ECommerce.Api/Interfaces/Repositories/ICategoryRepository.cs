using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Models;

namespace ECommerce.Api.Dtos.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task<bool> ExistsAsync(int id);
    Task<Category?> GetByIdAsync(int id);
    Task<PagedList<Category>> GetCategoriesAsync(PaginationParams paginationParams);
}