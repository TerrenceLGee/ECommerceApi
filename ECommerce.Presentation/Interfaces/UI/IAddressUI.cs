namespace ECommerce.Presentation.Interfaces.UI;

public interface IAddressUI
{
    Task<bool> HandleViewAllAddressesAsync();
    Task HandleViewAddressByIdAsync();
    Task HandleAddAddressAsync();
    Task HandleUpdateAddressAsync();
    Task HandleDeleteAddressAsync();
}