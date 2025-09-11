using System.ComponentModel.DataAnnotations;

namespace ECommerce.Presentation.Enums;

public enum AdminMenu
{
    [Display(Name = "View all available categories")]
    ViewAllCategories,
    [Display(Name = "View detailed information about a category")]
    ViewCategoryById,
    [Display(Name = "Add a category to the database")]
    AddCategory,
    [Display(Name = "Update an existing category in the database")]
    UpdateCategory,
    [Display(Name = "Delete an existing category in the database")]
    DeleteCategory,
    [Display(Name = "View all available products")]
    ViewAllProducts,
    [Display(Name = "View detailed information about a product")]
    ViewProductById,
    [Display(Name = "Add a product to the database")]
    AddProduct,
    [Display(Name = "Update an existing product in the database")]
    UpdateProduct,
    [Display(Name = "Delete an existing product in the database")]
    DeleteProduct,
    [Display(Name = "View all sales")]
    ViewSales,
    [Display(Name = "View sale by id")]
    ViewSaleById,
    [Display(Name = "Purchase products")]
    CreateSale,
    [Display(Name = "Update the status of an existing sale")]
    UpdateSale,
    [Display(Name = "Refund an existing sale")]
    RefundSale,
    [Display(Name = "Cancel an existing sale")]
    CancelSale,
    [Display(Name = "View all registered users in the system")]
    ViewAllUsers,
    [Display(Name = "View an individual user in the system")]
    ViewUserById,
    [Display(Name = "View all addresses of a user")]
    ViewUserAddress,
    [Display(Name = "Logout of the system")]
    Logout,
}