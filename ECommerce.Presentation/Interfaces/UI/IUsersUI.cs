namespace ECommerce.Presentation.Interfaces.UI;

public interface IUsersUI
{
    Task<bool> HandleViewAllUsersAsync();
    Task HandleViewUserByIdAsync();
    Task HandleViewUserAddressesAsync();
}