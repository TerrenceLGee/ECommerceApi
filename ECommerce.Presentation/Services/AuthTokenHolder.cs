using ECommerce.Presentation.Interfaces.Auth;

namespace ECommerce.Presentation.Services;

public class AuthTokenHolder : IAuthTokenHolder
{
    public string? Token { get; private set; }
    public void SetToken(string? token) => Token = token;
}