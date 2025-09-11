using ECommerce.Presentation.Dtos.Address.Request;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface IAddressService
{
    Task<AddressResponse?> AddAddressAsync(string customerId, CreateAddressRequest request);
    Task<AddressResponse?> UpdateAddressAsync(string customerId, int addressId, UpdateAddressRequest request);
    Task<AddressResponse?> DeleteAddressAsync(string customerId, int addressId);
    Task<PagedList<AddressResponse>?> GetAllAddressesAsync(string customerId, PaginationParams paginationParams);
    Task<AddressResponse?> GetAddressByIdAsync(string customerId, int addressId);
}