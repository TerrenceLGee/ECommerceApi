using ECommerce.Presentation.Dtos.Products.Response;
using ECommerce.Presentation.Interfaces;
using Spectre.Console;

namespace ECommerce.Presentation.UI.Operations.Products;

public class ProductsUI
{
    private readonly IProductsApiService _productsApiService;
    private readonly ICategoriesApiService _categoriesApiService;

    public ProductsUI(
        IProductsApiService productsApiService,
        ICategoriesApiService categoriesApiService)
    {
        _productsApiService = productsApiService;
        _categoriesApiService = categoriesApiService;
    }

    private void DisplayProducts(List<ProductResponse> products)
    {
        if (products.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no products available[/]");
            AnsiConsole.WriteLine("Press any key to return to the previous menu");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        var table = new Table()
            .Title($"[bold underline yellow]Products[/]")
            .Border(TableBorder.Rounded);

        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Description");
        table.AddColumn("Stock Keeping Unit");
        table.AddColumn("Price");
        table.AddColumn("Discount Percentage");
        table.AddColumn("Quantity In Stock");
        table.AddColumn("Is Active?");
        table.AddColumn("In Category");

        foreach (var product in products)
        {
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                product.Description ?? "N/A",
                product.StockKeepingUnit ?? "N/A",
                $"${product.Price:F2}",
                $"{(int)product.Discount}%",
                $"{product.StockQuantity}",
                $"{product.IsActive}",
                $"{product.Category.Name}");
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private void DisplayProductResponse(ProductResponse productResponse, string title)
    {
        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded);

        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Description");
        table.AddColumn("Stock Keeping Unit");
        table.AddColumn("Price");
        table.AddColumn("Discount Percentage");
        table.AddColumn("Quantity In Stock");
        table.AddColumn("Is Active?");
        table.AddColumn("In Category");

        var id = productResponse.Id.ToString();
        var name = productResponse.Name;
        var description = productResponse.Description ?? "N/A";
        var stockKeepingUnit = productResponse.StockKeepingUnit ?? "N/A";
        var price = $"${productResponse.Price:F2}";
        var discount = $"{(int)productResponse.Discount}%";
        var quantity = $"{productResponse.StockQuantity}";
        var isActive = $"{productResponse.IsActive}";
        var category = $"{productResponse.Category.Name}";

        table.AddRow(
            id,
            name,
            description,
            stockKeepingUnit,
            price,
            discount,
            quantity,
            isActive,
            category);

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private async Task<bool> DisplayCategorySummaries()
    {
        throw new NotImplementedException();
    }
}