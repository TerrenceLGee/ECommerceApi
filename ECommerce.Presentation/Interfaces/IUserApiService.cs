using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Auth.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface IUserApiService
{
    Task<Result<PagedList<UserResponse>>> GetAllUsersAsync(PaginationParams paginationParams);
    Task<Result<UserResponse?>> GetUserByIdAsync(string userId);
    Task<Result<PagedList<AddressResponse>>> GetUserAddressesByIdAsync(string userId, PaginationParams paginationParams);
}