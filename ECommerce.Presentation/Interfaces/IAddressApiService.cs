using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Request;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface IAddressApiService
{
    Task<Result<AddressResponse?>> AddAddressAsync(CreateAddressRequest request);
    Task<Result<AddressResponse?>> UpdateAddressAsync(int addressId, UpdateAddressRequest request);
    Task<Result<AddressResponse?>> DeleteAddressAsync(int addressId);
    Task<Result<PagedList<AddressResponse>?>> GetAllAddressesAsync(PaginationParams paginationParams);
    Task<Result<AddressResponse?>> GetAddressByIdAsync(int addressId);
}