using ECommerce.Presentation.Dtos.Categories.Response;
using ECommerce.Presentation.Dtos.Products.Response;
using ECommerce.Presentation.Interfaces;
using ECommerce.Presentation.UI.Helpers;
using Spectre.Console;

namespace ECommerce.Presentation.UI.Operations.Categories;

public class CategoriesUI
{
    private readonly ICategoriesApiService _categoryApiService;
    private readonly IProductsApiService _productsApiService;

    public CategoriesUI(
        ICategoriesApiService categoryApiService,
        IProductsApiService productsApiService)
    {
        _categoryApiService = categoryApiService;
        _productsApiService = productsApiService;
    }

    public async Task HandleViewAllCategories()
    {
        var countOfCategories = await _categoryApiService.GetCountOfCategoriesAsync();
        
        AnsiConsole.MarkupLine($"There are {countOfCategories} categories available to view");
        
        var pageNumber = AnsiConsole.Confirm("Would you like to specify the page number to view? (default is 1): ")
            ? AnsiConsole.Ask<int>("Enter page number to view")
            : 1;
        var pageSize = AnsiConsole.Confirm("Would you like to specify the max number of pages? (default is 10): ")
            ? AnsiConsole.Ask<int>("Enter number of pages to view")
            : 10;

        var response = await _categoryApiService.GetCategoriesAsync(pageNumber, pageSize);

        if (response.IsFailure || response.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{response.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        DisplayCategories(response.Value);
    }

    private void DisplayCategories(List<CategoryResponse> categories)
    {
        if (categories.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no categories available to view[]");
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        var table = new Table()
            .Title($"[bold underline yellow]Categories[/]")
            .Border(TableBorder.Rounded);

        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Description");
        table.AddColumn("Is Active?");

        foreach (var category in categories)
        {
            table.AddRow(
                category.Id.ToString(),
                category.Name,
                category.Description ?? "N/A",
                category.IsActive.ToString());

            if (category.Products.Count > 0)
            {
                DisplayProductsOfCategory(category.Products, $"Products under the category of {category.Name}");
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private async Task DisplayCategoryResponse(CategoryResponse response, string title)
    {
        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded);

        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Description");
        table.AddColumn("Is Active?");

        var id = $"{response.Id}";
        var name = $"{response.Name}";
        var description = $"{response.Description ?? "N/A"}";
        var isActive = $"{response.IsActive}";

        table.AddRow(id, name, description, isActive);
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        var countOfProducts = response.Products.Count;
        
        AnsiConsole.MarkupLine($"There are {countOfProducts} products available to view");
        
        var pageNumber = AnsiConsole.Confirm("Would you like to specify the page number to view? (default is 1): ")
            ? AnsiConsole.Ask<int>("Enter page number to view")
            : 1;
        var pageSize = AnsiConsole.Confirm("Would you like to specify the max number of pages? (default is 10): ")
            ? AnsiConsole.Ask<int>("Enter number of pages to view")
            : 10;
        
        var productsListResult = await _productsApiService.GetProductsAsync(pageNumber, pageSize);
        

        if (productsListResult.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]{productsListResult.ErrorMessage}[/]");
        }
        else
        {
            var productsList = productsListResult.Value!;

            if (productsList is null || productsList.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold green]Currently there are no products in this category[/]");
                return;
            }

            DisplayProductsOfCategory(productsList, $"Products under the category {response.Name}");
        }
    }

    private void DisplayProductsOfCategory(List<ProductResponse> products, string title)
    {
        var productsTable = new Table()
            .Title(title)
            .Border(TableBorder.Rounded);

        productsTable.AddColumn("Id");
        productsTable.AddColumn("Name");
        productsTable.AddColumn("Description");
        productsTable.AddColumn("Stock Keeping Unit");
        productsTable.AddColumn("Price");
        productsTable.AddColumn("Quantity in stock");

        foreach (var product in products)
        {
            productsTable.AddRow(
                product.Id.ToString(),
                product.Name,
                product.Description ?? "N/A",
                product.StockKeepingUnit ?? "N/A",
                $"${product.Price:F2}",
                product.StockQuantity.ToString());
        }

        AnsiConsole.Write(productsTable);
        AnsiConsole.WriteLine();
    }
}