using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Categories.Request;
using ECommerce.Presentation.Dtos.Categories.Response;
using ECommerce.Presentation.Dtos.Products.Response;
using ECommerce.Presentation.Interfaces;
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

    public async Task HandleViewAllCategoriesAsync()
    {
        var countOfCategoriesResult = await _categoryApiService.GetCountOfCategoriesAsync();

        if (countOfCategoriesResult.IsFailure || countOfCategoriesResult.Value == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no categories available to view[/]");
            AnsiConsole.WriteLine("Press any key to continue");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        AnsiConsole.MarkupLine($"There are {countOfCategoriesResult.Value} categories available to view");
        
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

    public async Task HandleViewCategoryByIdAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Select from the following categories to view:[/]");
        
        await HandleViewAllCategoriesAsync();

        var id = AnsiConsole.Ask<int>("[green]Enter the id of the category that you wish to view: [/]");

        var result = await _categoryApiService.GetCategoryByIdAsync(id);

        if (result.IsSuccess && result.Value is not null)
        {
            await DisplayCategoryResponse(result.Value, $"Category information");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
        }
    }

    public async Task HandleAddCategoryAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Add a category to the database[/]");
        AnsiConsole.WriteLine();

        var name = AnsiConsole.Ask<string>("Enter category name: ");
        var description = AnsiConsole.Ask<string>("Enter category description (optional): ");

        var request = new CreateCategoryRequest
        {
            Name = name,
            Description = description
        };

        Result<CategoryResponse?> categoryResponseResult = null!;

        await AnsiConsole.Status().StartAsync("Adding category...", async _ =>
        {
            categoryResponseResult = await _categoryApiService.CreateCategoryAsync(request);
        });

        if (categoryResponseResult.IsSuccess && categoryResponseResult.Value is not null)
        {
            var categoryResponse = categoryResponseResult.Value;
            
            AnsiConsole.MarkupLine("[bold green]Category added successfully![/]");
            var title = "Newly added category";
            await DisplayCategoryResponse(categoryResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold red]Failed to add category to the database: {categoryResponseResult.ErrorMessage}[/]");
        }
    }

    public async Task HandleUpdateCategoryAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Update an existing category in the database[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold underline green]Please choose from an existing category[/]");
        await HandleViewAllCategoriesAsync();
        AnsiConsole.WriteLine();

        var categoryId = AnsiConsole.Ask<int>("Enter the id of the category to update: ");

        var categoryToUpdateResult = await _categoryApiService.GetCategoryByIdAsync(categoryId);

        if (categoryToUpdateResult.IsFailure || categoryToUpdateResult.Value is null)
        {
            AnsiConsole.MarkupLine($"{categoryToUpdateResult.ErrorMessage}");
            AnsiConsole.MarkupLine("Press any key to continue");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        var categoryToUpdate = categoryToUpdateResult.Value;

        var name = AnsiConsole.Confirm("Update category name? ")
            ? AnsiConsole.Ask<string>("Enter updated name: ")
            : categoryToUpdate.Name;

        var description = AnsiConsole.Confirm("Update category description? ")
            ? AnsiConsole.Ask<string>("Enter updated description: ")
            : categoryToUpdate.Description;

        var isActive =
            AnsiConsole.Confirm($"Active status is currently {categoryToUpdate.IsActive}\nUpdate category status? ")
                ? AnsiConsole.Ask<bool>("Enter updated active status (true/false): ")
                : categoryToUpdate.IsActive;

        var request = new UpdateCategoryRequest
        {
            Name = name,
            Description = description,
            IsActive = isActive,
        };

        Result<CategoryResponse?> categoryResponseResult = null!;

        await AnsiConsole.Status().StartAsync("Updating category...", async _ =>
        {
            categoryResponseResult = await _categoryApiService.UpdateCategoryAsync(categoryId, request);
        });

        if (categoryResponseResult.IsSuccess && categoryResponseResult.Value is not null)
        {
            var categoryResponse = categoryResponseResult.Value;

            AnsiConsole.MarkupLine("[bold green]Category updated successfully![/]");
            var title = "Updated category";
            await DisplayCategoryResponse(categoryResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Failed to update category in the database: {categoryResponseResult.ErrorMessage}[/]");
        }
    }

    public async Task HandleDeleteCategoryAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Delete an existing category from the database[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[bold underline green]Please choose from an existing category: [/]");
        await HandleViewAllCategoriesAsync();
        AnsiConsole.WriteLine();

        var categoryId = AnsiConsole.Ask<int>("Enter id of category to delete: ");

        var categoryToDeleteResult = await _categoryApiService.GetCategoryByIdAsync(categoryId);

        if (categoryToDeleteResult.IsFailure || categoryToDeleteResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{categoryToDeleteResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to continue");
            Console.ReadKey();
            return;
        }

        var categoryToDelete = categoryToDeleteResult.Value;

        var confirmDeleteCategory = AnsiConsole.Confirm($"Are you sure you wish to delete {categoryToDelete.Name}? ");

        if (!confirmDeleteCategory)
        {
            AnsiConsole.MarkupLine($"[bold red]Category {categoryToDelete.Name} will NOT be deleted[/]");
            AnsiConsole.WriteLine("Press any key to continue");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        Result<CategoryResponse?> categoryResponseResult = null!;

        await AnsiConsole.Status().StartAsync("Deleting category...", async _ =>
        {
            categoryResponseResult = await _categoryApiService.DeleteCategoryAsync(categoryId);
        });

        if (categoryResponseResult.IsSuccess && categoryResponseResult.Value is not null)
        {
            var categoryResponse = categoryResponseResult.Value;
            AnsiConsole.MarkupLine("[bold green]Category deleted successfully![/]");
            var title = "Deleted category";
            await DisplayCategoryResponse(categoryResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold red]{categoryResponseResult.ErrorMessage}[/]");
        }
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