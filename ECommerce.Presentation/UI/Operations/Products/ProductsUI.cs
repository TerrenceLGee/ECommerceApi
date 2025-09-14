using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Products.Request;
using ECommerce.Presentation.Dtos.Products.Response;
using ECommerce.Presentation.Enums;
using ECommerce.Presentation.Enums.Extensions;
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

    public async Task<bool> HandleViewAllProductsAsync()
    {
        var countOfProductsResult = await _productsApiService.GetCountOfProductsAsync();

        if (countOfProductsResult.IsFailure || countOfProductsResult.Value == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no products available to view[/]");
            AnsiConsole.WriteLine("Press any key to continue");
            Console.ReadKey();
            AnsiConsole.Clear();
            return false;
        }
        
        AnsiConsole.MarkupLine($"There are {countOfProductsResult.Value} products available to view");

        var pageNumber = AnsiConsole.Confirm("Would you like to specify the page number to view? (default is 1): ")
            ? AnsiConsole.Ask<int>("Enter page number to view ")
            : 1;

        var pageSize = AnsiConsole.Confirm("Would you like to specify the max number of pages? (default is 10): ")
            ? AnsiConsole.Ask<int>("Enter number of pages to view")
            : 10;
        
        var response = await _productsApiService.GetProductsAsync(pageNumber, pageSize);

        if (response.IsFailure || response.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{response.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return false;
        }
        
        DisplayProducts(response.Value);
        return true;
    }

    public async Task HandleViewProductByIdAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View detailed information about a product[/]");
        AnsiConsole.MarkupLine("[green]Choose a product to view its information[/]");

        if (!await HandleViewAllProductsAsync())
        {
            return;
        }

        var id = AnsiConsole.Ask<int>("[green]Enter the id of the product that you wish to view[/]");

        var productResult = await _productsApiService.GetProductByIdAsync(id);

        if (productResult.IsFailure || productResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{productResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to return to the previous menu: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        DisplayProductResponse(productResult.Value,$"Product #{productResult.Value.Id}");
    }

    public async Task ViewProductForSale(int id)
    {
        var productResult = await _productsApiService.GetProductByIdAsync(id);

        if (productResult.IsFailure || productResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{productResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to continue: ");
            Console.ReadKey();
            return;
        }
        
        DisplayProductResponse(productResult.Value, $"Product #{productResult.Value.Id}");
    }
    public async Task HandleAddProductAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Add a new product to the database[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[underline green]Choose a category for the new product:[/]");
        if (!await DisplayCategorySummariesAsync())
        {
            return;
        }
        var categoryId = AnsiConsole.Ask<int>("Enter the id of the category to add this product under: ");

        var name = AnsiConsole.Ask<string>("Enter product name: ");
        var description = AnsiConsole.Ask<string?>("Enter description (optional): ");
        var stockKeepingUnit = AnsiConsole.Ask<string?>("Enter Stock Keeping Unit (optional): ");
        var price = AnsiConsole.Ask<decimal>("Enter product price: ");
        var discount = GetProductDiscount();
        var quantity = AnsiConsole.Ask<int>("Enter the quantity of the product being added: ");
        var isActive = AnsiConsole.Ask<bool>("Mark this product as active? (true/false): ");

        var request = new CreateProductRequest
        {
            Name = name,
            Description = description,
            StockKeepingUnit = stockKeepingUnit,
            Price = price,
            Discount = discount,
            StockQuantity = quantity,
            CategoryId = categoryId,
            IsActive = isActive,
        };

        Result<ProductResponse?> productResponseResult = null!;

        await AnsiConsole.Status().StartAsync("Adding product...", async _ =>
        {
            productResponseResult = await _productsApiService.CreateProductAsync(request);
        });

        if (productResponseResult.IsSuccess && productResponseResult.Value is not null)
        {
            var productResponse = productResponseResult.Value;
            AnsiConsole.MarkupLine("[bold green]Product added successfully![/]");
            var title = "Newly added product";
            DisplayProductResponse(productResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{productResponseResult.ErrorMessage}[/]");
        }
    }

    public async Task HandleUpdateProductAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Update a product in the database[/]");
        AnsiConsole.MarkupLine("[green]Choose from the available products:[/]");

        if (!await HandleViewAllProductsAsync())
        {
            return;
        }

        var productId = AnsiConsole.Ask<int>("Enter the id of the product to update: ");

        var productResult = await _productsApiService.GetProductByIdAsync(productId);

        if (productResult.IsFailure || productResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{productResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to return to the previous menu: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        var productToUpdate = productResult.Value;

        var updateProductCategory = AnsiConsole.Confirm("Do you wish to update/change the product's category? ");
        int categoryId;
        
        if (updateProductCategory)
        {
            AnsiConsole.MarkupLine("[green]Choose from the following categories:[/]");
            if (!await DisplayCategorySummariesAsync())
            {
                return;
            }

            categoryId = AnsiConsole.Ask<int>("Enter the updated category id: ");
        }
        else
        {
            categoryId = productToUpdate.Category.Id;
        }

        var name = AnsiConsole.Confirm("Update product name? ")
            ? AnsiConsole.Ask<string>("Enter updated product name: ")
            : productToUpdate.Name;

        var description = AnsiConsole.Confirm("Update product description? ")
            ? AnsiConsole.Ask<string?>("Enter updated product description: ")
            : productToUpdate.Description;

        var stockKeepingUnit = AnsiConsole.Confirm("Update product Stock Keeping Unit? ")
            ? AnsiConsole.Ask<string?>("Enter updated Stock Keeping Unit: ")
            : productToUpdate.StockKeepingUnit;
        var quantity = AnsiConsole.Confirm("Update product quantity? ")
            ? AnsiConsole.Ask<int>("Enter updated product quantity: ")
            : productToUpdate.StockQuantity;

        var updateDiscountStatus = AnsiConsole.Confirm("Do you wish to update the discount on the product? ");

        DiscountStatus discount;
        if (updateDiscountStatus)
        {
            discount = GetProductDiscount();
        }
        else
        {
            discount = productToUpdate.Discount;
        }

        var isActive =
            AnsiConsole.Confirm($"Update product status? Currently it is: {productToUpdate.IsActive}")
                ? AnsiConsole.Ask<bool>("Enter updated product active status (true/false): ")
                : productToUpdate.IsActive;

        var request = new UpdateProductRequest
        {
            Name = name,
            Description = description,
            StockKeepingUnit = stockKeepingUnit,
            StockQuantity = quantity,
            Discount = discount,
            CategoryId = categoryId,
            IsActive = isActive
        };

        Result<ProductResponse?> productResponseResult = null!;

        await AnsiConsole.Status().StartAsync("Updating product...", async _ =>
        {
            productResponseResult = await _productsApiService.UpdateProductAsync(productId, request);
        });

        if (productResponseResult.IsSuccess && productResponseResult.Value is not null)
        {
            var productResponse = productResponseResult.Value;
            AnsiConsole.MarkupLine("[bold green]Product added successfully![/]");
            var title = "Updated product";
            DisplayProductResponse(productResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{productResponseResult.ErrorMessage}[/]");
        }
    }

    public async Task HandleDeleteProductAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Delete a product from the database[/]");
        AnsiConsole.MarkupLine("[green]Choose from the available products[/]");

        if (!await HandleViewAllProductsAsync())
        {
            return;
        }

        var productId = AnsiConsole.Ask<int>("Enter the id of the product to delete: ");

        var productToDeleteResult = await _productsApiService.GetProductByIdAsync(productId);

        if (productToDeleteResult.IsFailure || productToDeleteResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{productToDeleteResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to return to the previous menu: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        Result<ProductResponse?> productToDeleteResponse = null!;

        await AnsiConsole.Status().StartAsync("Deleting product...", async _ =>
        {
            productToDeleteResponse = await _productsApiService.DeleteProductAsync(productId);
        });

        if (productToDeleteResponse.IsSuccess && productToDeleteResponse.Value is not null)
        {
            var productResponse = productToDeleteResponse.Value;
            AnsiConsole.MarkupLine("[bold green]Product deleted successfully![/]");
            var title = "Deleted product";
            DisplayProductResponse(productResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{productToDeleteResponse.ErrorMessage}[/]");
        }
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

    private async Task<bool> DisplayCategorySummariesAsync()
    {
        var categoriesResult = await _categoriesApiService.GetCategoriesForSummaryAsync();

        if (categoriesResult.IsFailure || categoriesResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{categoriesResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to continue");
            Console.ReadKey();
            AnsiConsole.Clear();
            return false;
        }

        var categories = categoriesResult.Value
            .Where(c => c.IsActive)
            .ToList();

        var table = new Table()
            .Title("Available categories")
            .Border(TableBorder.Rounded);

        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Description");

        foreach (var category in categories)
        {
            table.AddRow(
                category.Id.ToString(),
                category.Name,
                category.Description ?? "N/A");
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        return true;
    }

    private DiscountStatus GetProductDiscount()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<DiscountStatus>()
                .Title("Please choose discount (if any): ")
                .AddChoices(Enum.GetValues<DiscountStatus>())
                .UseConverter(choice => choice.GetDisplayName()));
    }
}