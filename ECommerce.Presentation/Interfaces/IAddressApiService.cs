using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Request;
using ECommerce.Presentation.Dtos.Address.Response;

namespace ECommerce.Presentation.Interfaces;

public interface IAddressApiService
{
    Task<Result<AddressResponse?>> AddAddressAsync(CreateAddressRequest request);
    Task<Result<AddressResponse?>> UpdateAddressAsync(int addressId, UpdateAddressRequest request);
    Task<Result<AddressResponse?>> DeleteAddressAsync(int addressId);
    Task<Result<List<AddressResponse>?>> GetAllAddressesAsync(int pageNumber, int pageSize);
    Task<Result<AddressResponse?>> GetAddressByIdAsync(int addressId);
    Task<Result<int>> GetCountOfAddressesAsync();
}