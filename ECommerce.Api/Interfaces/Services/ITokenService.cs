using ECommerce.Api.Identity;

namespace ECommerce.Api.Interfaces.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}