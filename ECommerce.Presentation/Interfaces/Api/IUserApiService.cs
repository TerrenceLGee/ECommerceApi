using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Auth.Response;

namespace ECommerce.Presentation.Interfaces.Api;

public interface IUserApiService
{
    Task<Result<List<UserResponse>>> GetAllUsersAsync(int pageNumber, int pageSize);
    Task<Result<UserResponse?>> GetUserByIdAsync(string userId);
    Task<Result<List<AddressResponse>>> GetUserAddressesByIdAsync(string userId, int pageNumber, int pageSize);
    Task<Result<int>> GetCountOfUsersAsync();
}