using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Models;

namespace ECommerce.Api.Interfaces.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<Product?> GetByIdAsync(int id);
    Task<PagedList<Product>> GetAllAsync(PaginationParams paginationParams);
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);
}