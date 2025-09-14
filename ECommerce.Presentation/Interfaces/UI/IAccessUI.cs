namespace ECommerce.Presentation.Interfaces.UI;

public interface IAccessUI
{
    Task HandleRegistration();
    Task<(bool isLoggedIn, bool isAdmin)> HandleLogin();
}