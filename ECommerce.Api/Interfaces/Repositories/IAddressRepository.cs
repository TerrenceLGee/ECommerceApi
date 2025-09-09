using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Models;

namespace ECommerce.Api.Interfaces.Repositories;

public interface IAddressRepository
{
    Task AddAddressAsync(Address address);
    Task UpdateAddressAsync(Address address);
    Task DeleteAddressAsync(Address address);
    Task<PagedList<Address>> GetAllAsync(string customerId, PaginationParams paginationParams);
    Task<Address?> GetAddressById(string customerId, int addressId);
}