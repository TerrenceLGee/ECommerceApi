namespace ECommerce.Presentation.Interfaces.UI;

public interface IProductsUI
{
    Task<bool> HandleViewAllProductsAsync();
    Task HandleViewProductByIdAsync();
    Task ViewProductForSale(int id);
    Task HandleAddProductAsync();
    Task HandleUpdateProductAsync();
    Task HandleDeleteProductAsync();
}