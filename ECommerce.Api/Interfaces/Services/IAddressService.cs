using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Address.Request;
using ECommerce.Api.Dtos.Address.Response;
using ECommerce.Api.Dtos.Shared.Pagination;

namespace ECommerce.Api.Interfaces.Services;

public interface IAddressService
{
    Task<Result<AddressResponse>> AddAddressAsync(string customerId, CreateAddressRequest request);
    Task<Result<AddressResponse>> UpdateAddressAsync(string customerId, int addressId,  UpdateAddressRequest request);
    Task<Result<AddressResponse>> DeleteAddressAsync(string customerId, int addressId);
    Task<Result<PagedList<AddressResponse>>> GetAllAddressesAsync(string customerId, PaginationParams paginationParams);
    Task<Result<AddressResponse>> GetAddressByIdAsync(string customerId, int addressId);
}