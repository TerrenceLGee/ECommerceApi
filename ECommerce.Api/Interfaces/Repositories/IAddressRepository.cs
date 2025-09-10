using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Models;

namespace ECommerce.Api.Interfaces.Repositories;

public interface IAddressRepository
{
    Task AddAddressAsync(Address address);
    Task UpdateAddressAsync(Address address);
    Task DeleteAddressAsync(Address address);
    Task<PagedList<Address>> GetAllAddressesAsync(string customerId, PaginationParams paginationParams);
    Task<Address?> GetAddressByIdAsync(string customerId, int addressId);
}