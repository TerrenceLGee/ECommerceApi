namespace ECommerce.Presentation.Interfaces.UI;

public interface ICategoriesUI
{
    Task<bool> HandleViewAllCategoriesAsync();
    Task HandleViewCategoryByIdAsync();
    Task HandleAddCategoryAsync();
    Task HandleUpdateCategoryAsync();
    Task HandleDeleteCategoryAsync();
}