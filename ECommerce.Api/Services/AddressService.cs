using System.Data.Common;
using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Address.Request;
using ECommerce.Api.Dtos.Address.Response;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Mappings;

namespace ECommerce.Api.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly ILogger<AddressService> _logger;

    public AddressService(
        IAddressRepository addressRepository,
        ILogger<AddressService> logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public async Task<Result<AddressResponse>> AddAddressAsync(string customerId, CreateAddressRequest request)
    {
        try
        {
            var address = request.MapFromCreateAddressRequestToAddress();
            address.ApplicationUserId = customerId;
            await _addressRepository.AddAddressAsync(address);
            return Result<AddressResponse>.Ok(address.MapAddressToAddressResponse());
        }
        catch (DbException ex)
        {
            _logger.LogError("There was an error adding the address: {errorMessage}", ex.Message);
            return Result<AddressResponse>.Fail($"There was an error adding the address: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<AddressResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse>> UpdateAddressAsync(string customerId, int addressId, UpdateAddressRequest request)
    {
        try
        {
            var addressToUpdate = await _addressRepository.GetAddressByIdAsync(customerId, addressId);

            if (addressToUpdate is null)
            {
                _logger.LogError("Address with Id {id} not found", addressId);
                return Result<AddressResponse>.Fail($"Address with Id {addressId} not found");
            }
            
            request.MapFromUpdateAddressRequestToAddress(addressToUpdate);
            await _addressRepository.UpdateAddressAsync(addressToUpdate);

            return Result<AddressResponse>.Ok(addressToUpdate.MapAddressToAddressResponse());
        }
        catch (DbException ex)
        {
            _logger.LogCritical("There was an error updating the address: {errorMessage}", ex.Message);
            return Result<AddressResponse>.Fail($"There was an error updating the address: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<AddressResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse>> DeleteAddressAsync(string customerId, int addressId)
    {
        try
        {
            var addressToDelete = await _addressRepository.GetAddressByIdAsync(customerId, addressId);

            if (addressToDelete is null)
            {
                _logger.LogError("Address with Id {id} not found", addressId);
                return Result<AddressResponse>.Fail($"Address with Id {addressId} not found");
            }

            await _addressRepository.DeleteAddressAsync(addressToDelete);
            return Result<AddressResponse>.Ok(addressToDelete.MapAddressToAddressResponse());
        }
        catch (DbException ex)
        {
            _logger.LogCritical("There was an error deleting the address: {errorMessage}", ex.Message);
            return Result<AddressResponse>.Fail($"There was an error adding the deleting: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<AddressResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<AddressResponse>>> GetAllAddressesAsync(string customerId, PaginationParams paginationParams)
    {
        try
        {
            var addresses = await _addressRepository.GetAllAddressesAsync(customerId, paginationParams);

            var addressResponseDtos = addresses.Items
                .Select(a => a.MapAddressToAddressResponse())
                .ToList();

            var pagedResponse = new PagedList<AddressResponse>(
                addressResponseDtos,
                addresses.TotalCount,
                addresses.CurrentPage,
                addresses.PageSize);

            return Result<PagedList<AddressResponse>>.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving all addresses: {errorMessage}", ex.Message);
            return Result<PagedList<AddressResponse>>.Fail($"There was an error retrieving all addresses: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<PagedList<AddressResponse>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<AddressResponse>> GetAddressByIdAsync(string customerId, int addressId)
    {
        try
        {
            var address = await _addressRepository.GetAddressByIdAsync(customerId, addressId);

            if (address is null)
            {
                _logger.LogError("Address with Id {id} not found.", addressId);
                return Result<AddressResponse>.Fail($"Address with Id {addressId} not found");
            }

            return Result<AddressResponse>.Ok(address.MapAddressToAddressResponse());
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving the address: {errorMessage}", ex.Message);
            return Result<AddressResponse>.Fail($"There was an error retrieving the address: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<AddressResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetCountOfAddressesAsync(string customerId)
    {
        try
        {
            var count = await _addressRepository.GetCountOfAddressesAsync(customerId);

            return Result<int>.Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error retrieving the count of addresses from the database: {errorMessage}", ex.Message);
            return Result<int>.Fail(
                $"There was an unexpected error retrieving the count of addresses from the database: {ex.Message}");
        }
    }
}