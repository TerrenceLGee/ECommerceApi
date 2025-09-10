using ECommerce.Api.Common.Results;
using ECommerce.Api.Data;
using ECommerce.Api.Dtos.Address.Response;
using ECommerce.Api.Dtos.Auth.Response;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Identity;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Mappings;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    // private readonly ApplicationDbContext _context;
    private readonly IAddressService _addressService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager, 
        IAddressService addressService,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _addressService = addressService;
        _logger = logger;
    }
    
    public async Task<Result<PagedList<UserResponse>>> GetAllUsersAsync(PaginationParams paginationParams)
    {
        try
        {
            var query = _userManager.Users
                .Include(u => u.Addresses)
                .AsQueryable();

            query = paginationParams.OrderBy switch
            {
                "birthDateAsc" => query.OrderBy(u => u.DateOfBirth),
                "birthDateDesc" => query.OrderByDescending(u => u.DateOfBirth),
                "firstNameAsc" => query.OrderBy(u => u.FirstName),
                "firstNameDesc" => query.OrderByDescending(u => u.FirstName),
                "lastNameAsc" => query.OrderBy(u => u.LastName),
                "lastNameDesc" => query.OrderByDescending(u => u.LastName),
                "userIdDescending" => query.OrderByDescending(u => u.Id),
                _ => query.OrderBy(u => u.Id)
            };

            var pagedUsers = await
                PagedList<ApplicationUser>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);

            var userResponseDtos = pagedUsers.Items
                .Select(u => u.MapApplicationUserToUserResponse())
                .ToList();

            var pagedResponse = new PagedList<UserResponse>(
                userResponseDtos,
                pagedUsers.TotalCount,
                pagedUsers.CurrentPage,
                pagedUsers.PageSize);

            return Result<PagedList<UserResponse>>.Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error retrieving users from the database: {errorMessage}", ex.Message);
            return Result<PagedList<UserResponse>>.Fail(
                $"There was an unexpected error retrieving users from the database: {ex.Message}");
        }
    }

    public async Task<Result<UserResponse>> GetUserByIdAsync(string userId)
    {
        try
        {
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                _logger.LogCritical("User with Id {userId} not found.", userId);
                return Result<UserResponse>.Fail($"User with Id {userId} not found");
            }

            return Result<UserResponse>.Ok(user.MapApplicationUserToUserResponse());
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error retrieving user with id {userId} from the database: {errorMessage}", userId, ex.Message);
            return Result<UserResponse>.Fail(
                $"There was an unexpected error retrieving user with id {userId} from the database: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<AddressResponse>>> GetUserAddressesByIdAsync(string userId, PaginationParams paginationParams)
    {
        try
        {
            // var query = _context.Addresses
            //     .Include(a => a.Customer)
            //     .Where(a => a.AddressType == userId)
            //     .AsQueryable();
            //
            // if (!string.IsNullOrEmpty(paginationParams.Filter))
            // {
            //     query = query.Where(a =>
            //         a.AddressType != null && a.AddressType.ToLower() == paginationParams.Filter.ToLower());
            // }
            //
            // query = paginationParams.OrderBy switch
            // {
            //     "idAsc" => query.OrderBy(a => a.Id),
            //     "idDesc" => query.OrderByDescending(a => a.Id),
            //     _ => query.OrderBy(a => a.AddressType)
            // };
            //
            // var pagedAddresses =
            //     await PagedList<Address>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
            //
            // var addressResponseDtos = pagedAddresses.Items
            //     .Select(a => a.MapAddressToAddressResponse())
            //     .ToList();
            //
            // var pagedResponse = new PagedList<AddressResponse>(
            //     addressResponseDtos,
            //     pagedAddresses.TotalCount,
            //     pagedAddresses.CurrentPage,
            //     pagedAddresses.PageSize);

            return  await _addressService.GetAllAddressesAsync(userId, paginationParams);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error retrieving the addresses of user with id {userId} from the database: {errorMessage}", userId, ex.Message);
            return Result<PagedList<AddressResponse>>.Fail(
                $"There was an unexpected error retrieving the addresses of user with id {userId} from the database: {ex.Message}");
        }
    }
}