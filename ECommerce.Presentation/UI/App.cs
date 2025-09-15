using ECommerce.Presentation.Enums;
using ECommerce.Presentation.Enums.Extensions;
using ECommerce.Presentation.Interfaces.UI;
using Spectre.Console;

namespace ECommerce.Presentation.UI;

public class App : IApp
{
    private readonly IAccessUI _access;
    private readonly IProductsUI _products;
    private readonly ICategoriesUI _categories;
    private readonly ISalesUI _sales;
    private readonly IAddressUI _addresses;
    private readonly IUsersUI _users;

    public App(
        IAccessUI access,
        IProductsUI products,
        ICategoriesUI categories,
        ISalesUI sales,
        IAddressUI addresses,
        IUsersUI users)
    {
        _access = access;
        _products = products;
        _categories = categories;
        _sales = sales;
        _addresses = addresses;
        _users = users;
    }

    public async Task RunAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Welcome to the E-Commerce Client![/]");
        
        var exitSystem = false;

        while (!exitSystem)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<StartMenu>()
                    .Title("Please choose one of the following options:")
                    .AddChoices(Enum.GetValues<StartMenu>())
                    .UseConverter(choice => choice.GetDisplayName()));

            switch (choice)
            {
                case StartMenu.GoToHomePage:
                    await LoginLoop();
                    break;
                case StartMenu.ExitProgram:
                    exitSystem = true;
                    break;
            }
        }

        AnsiConsole.MarkupLine("\n[yellow]Goodbye![/]");
        ClearTheScreen();
    }

    private async Task LoginLoop()
    {
        var isLoggedIn = false;
        var isAdmin = false;

        while (!isLoggedIn)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<HomeMenu>()
                    .Title("Please choose one of the following options:")
                    .AddChoices(Enum.GetValues<HomeMenu>()
                    )
                    .UseConverter(choice => choice.GetDisplayName()));

            switch (choice)
            {
                case HomeMenu.Login:
                    (isLoggedIn, isAdmin) = await _access.HandleLogin();
                    break;
                case HomeMenu.Register:
                    await _access.HandleRegistration();
                    break;
                case HomeMenu.Logout:
                    AnsiConsole.MarkupLine("[yellow]Logging out...[/]");
                    return;
            }

            if (isAdmin)
            {
                await AdminLoop();
            }
            else
            {
                await CustomerLoop();
            }

            AnsiConsole.MarkupLine("\n[yellow]You have been logged out.[/]");
        }
        ClearTheScreen();
    }

    private async Task AdminLoop()
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<AdminMenu>()
                    .Title("\n[green]Admin Memu[/]")
                    .AddChoices(Enum.GetValues<AdminMenu>())
                    .UseConverter(m => m.GetDisplayName()));

            switch (choice)
            {
                case AdminMenu.ViewAllCategories:
                    await _categories.HandleViewAllCategoriesAsync();
                    break;
                case AdminMenu.ViewCategoryById:
                    await _categories.HandleViewCategoryByIdAsync();
                    break;
                case AdminMenu.AddCategory:
                    await _categories.HandleAddCategoryAsync();
                    break;
                case AdminMenu.UpdateCategory:
                    await _categories.HandleUpdateCategoryAsync();
                    break;
                case AdminMenu.DeleteCategory:
                    await _categories.HandleDeleteCategoryAsync();
                    break;
                case AdminMenu.ViewAllProducts:
                    await _products.HandleViewAllProductsAsync();
                    break;
                case AdminMenu.ViewProductById:
                    await _products.HandleViewProductByIdAsync();
                    break;
                case AdminMenu.AddProduct:
                    await _products.HandleAddProductAsync();
                    break;
                case AdminMenu.UpdateProduct:
                    await _products.HandleUpdateProductAsync();
                    break;
                case AdminMenu.DeleteProduct:
                    await _products.HandleDeleteProductAsync();
                    break;
                case AdminMenu.ViewSales:
                    await _sales.HandleViewAllSalesAsync();
                    break;
                case AdminMenu.ViewSaleById:
                    await _sales.HandleViewSaleByIdAsync();
                    break;
                case AdminMenu.CreateSale:
                    await _sales.HandleCreateSaleAsync();
                    break;
                case AdminMenu.UpdateSale:
                    await _sales.HandleUpdateSaleStatusAsync();
                    break;
                case AdminMenu.RefundSale:
                    await _sales.HandleRefundSaleAsync();
                    break;
                case AdminMenu.CancelSale:
                    await _sales.HandleAdminCancelSaleAsync();
                    break;
                case AdminMenu.ViewAllUsers:
                    await _users.HandleViewAllUsersAsync();
                    break;
                case AdminMenu.ViewUserById:
                    await _users.HandleViewUserByIdAsync();
                    break;
                case AdminMenu.ViewUserAddress:
                    await _users.HandleViewUserAddressesAsync();
                    break;
                case AdminMenu.Logout:
                    return;
            }
            ClearTheScreen();
        }
    }

    private async Task CustomerLoop()
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<CustomerMenu>()
                    .Title("\n[green]Customer Menu[/]")
                    .AddChoices(Enum.GetValues<CustomerMenu>())
                    .UseConverter(m => m.GetDisplayName()));

            switch (choice)
            {
                case CustomerMenu.ViewAllCategories:
                    await _categories.HandleViewAllCategoriesAsync();
                    break;
                case CustomerMenu.ViewCategoryById:
                    await _categories.HandleViewCategoryByIdAsync();
                    break;
                case CustomerMenu.ViewAllProducts:
                    await _products.HandleViewAllProductsAsync();
                    break;
                case CustomerMenu.ViewProductById:
                    await _products.HandleViewProductByIdAsync();
                    break;
                case CustomerMenu.ViewMySales:
                    await _sales.HandleViewAllSalesForUserAsync();
                    break;
                case CustomerMenu.ViewSaleById:
                    await _sales.HandleViewUserSaleByIdAsync();
                    break;
                case CustomerMenu.CreateSale:
                    await _sales.HandleCreateSaleAsync();
                    break;
                case CustomerMenu.CancelSale:
                    await _sales.HandleUserCancelSaleAsync();
                    break;
                case CustomerMenu.ViewAddresses:
                    await _addresses.HandleViewAllAddressesAsync();
                    break;
                case CustomerMenu.ViewAddressById:
                    await _addresses.HandleViewAddressByIdAsync();
                    break;
                case CustomerMenu.AddAddress:
                    await _addresses.HandleAddAddressAsync();
                    break;
                case CustomerMenu.UpdateAddress:
                    await _addresses.HandleUpdateAddressAsync();
                    break;
                case CustomerMenu.DeleteAddress:
                    await _addresses.HandleDeleteAddressAsync();
                    break;
                case CustomerMenu.Logout:
                    return;
            }
            ClearTheScreen();
        }
    }

    private void ClearTheScreen()
    {
        AnsiConsole.MarkupLine("[blue]Press any key to continue[/]");
        Console.ReadKey();
        AnsiConsole.Clear();
    }
}