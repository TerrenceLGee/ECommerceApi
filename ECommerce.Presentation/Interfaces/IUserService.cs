using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Auth.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface IUserService
{
    Task<PagedList<UserResponse>> GetAllUsersAsync(PaginationParams paginationParams);
    Task<UserResponse?> GetUserByIdAsync(string userId);
    Task<PagedList<AddressResponse>> GetUserAddressByIdAsync(string userId, PaginationParams paginationParams);
}