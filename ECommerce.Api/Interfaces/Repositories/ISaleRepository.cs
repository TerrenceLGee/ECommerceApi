using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Models;

namespace ECommerce.Api.Interfaces.Repositories;

public interface ISaleRepository
{
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task<Sale?> GetByIdAsync(int id);
    Task<Sale?> GetUserSaleByIdAsync(string userId, int saleId);
    Task<PagedList<Sale>> GetAllAsync(PaginationParams paginationParams);
    Task<PagedList<Sale>> GetByUserIdAsync(string userId, PaginationParams paginationParams);
}