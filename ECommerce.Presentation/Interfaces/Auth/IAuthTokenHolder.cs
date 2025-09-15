namespace ECommerce.Presentation.Interfaces.Auth;

public interface IAuthTokenHolder
{
    string? Token { get; }
    void SetToken(string? token);
}