using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Address.Response;
using ECommerce.Api.Dtos.Auth.Response;
using ECommerce.Api.Dtos.Shared.Pagination;

namespace ECommerce.Api.Interfaces.Services;

public interface IUserService
{
    Task<Result<PagedList<UserResponse>>> GetAllUsersAsync(PaginationParams paginationParams);
    Task<Result<UserResponse>> GetUserByIdAsync(string userId);
    Task<Result<PagedList<AddressResponse>>> GetUserAddressesByIdAsync(string userId, PaginationParams paginationParams);
    Task<Result<int>> GetUserCountAsync();
}