using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Request;
using ECommerce.Presentation.Dtos.Address.Response;

namespace ECommerce.Presentation.Interfaces.Api;

public interface IAddressApiService
{
    Task<Result<AddressResponse?>> AddAddressAsync(CreateAddressRequest request);
    Task<Result<AddressResponse?>> UpdateAddressAsync(int addressId, UpdateAddressRequest request);
    Task<Result<AddressResponse?>> DeleteAddressAsync(int addressId);
    Task<Result<List<AddressResponse>?>> GetAllAddressesAsync(int pageNumber, int pageSize);
    Task<Result<List<AddressResponse>?>> GetAddressesForDisplay();
    Task<Result<AddressResponse?>> GetAddressByIdAsync(int addressId);
    Task<Result<int>> GetCountOfAddressesAsync();
}