using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Sales.Request;
using ECommerce.Presentation.Dtos.Sales.Response;
using ECommerce.Presentation.Enums;
using ECommerce.Presentation.Enums.Extensions;
using ECommerce.Presentation.Interfaces;
using ECommerce.Presentation.UI.Operations.Addresses;
using ECommerce.Presentation.UI.Operations.Products;
using Spectre.Console;

namespace ECommerce.Presentation.UI.Operations.Sales;

public class SalesUI
{
    private readonly ISalesApiService _salesApiService;
    private readonly IProductsApiService _productsApiService;
    private readonly ICategoriesApiService _categoriesApiService;
    private readonly IAddressApiService _addressApiService;
    private const string DateFormat = "MM/dd/yyyy hh:mm:ss";

    public SalesUI(
        ISalesApiService salesApiService,
        IProductsApiService productsApiService,
        ICategoriesApiService categoriesApiService,
        IAddressApiService addressApiService)
    {
        _salesApiService = salesApiService;
        _productsApiService = productsApiService;
        _categoriesApiService = categoriesApiService;
        _addressApiService = addressApiService;
    }

    public async Task HandleViewAllSalesAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View All Sales[/]");
        var salesCountResult = await _salesApiService.GetCountOfSalesAsync();

        if (salesCountResult.IsFailure || salesCountResult.Value == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no sales available to display[/]");
            AnsiConsole.WriteLine("Press any key to continue: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        AnsiConsole.MarkupLine($"There are currently [green]{salesCountResult.Value}[/] sales available to view");

        var pageNumber = AnsiConsole.Confirm("Would you like to specify the page number to view? (default is 1): ")
            ? AnsiConsole.Ask<int>("Enter page number to view: ")
            : 1;

        var pageSize = AnsiConsole.Confirm("Would you like to specify the max number of pages? (default is 10): ")
            ? AnsiConsole.Ask<int>("Enter number of pages to view: ")
            : 10;

        var response = await _salesApiService.GetAllSalesAsync(pageNumber, pageSize);

        if (response.IsFailure || response.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{response.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        DisplaySaleResponses(response.Value);
    }

    public async Task HandleViewAllSalesForUserAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View All Sales[/]");
        var salesCountResult  = await _salesApiService.GetCountOfSalesForUserAsync();
        
        if (salesCountResult.IsFailure || salesCountResult.Value == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no sales available to display[/]");
            AnsiConsole.WriteLine("Press any key to continue: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        AnsiConsole.MarkupLine($"There are currently [green]{salesCountResult.Value}[/] sales available to view");

        var pageNumber = AnsiConsole.Confirm("Would you like to specify the page number to view? (default is 1): ")
            ? AnsiConsole.Ask<int>("Enter page number to view: ")
            : 1;

        var pageSize = AnsiConsole.Confirm("Would you like to specify the max number of pages? (default is 10): ")
            ? AnsiConsole.Ask<int>("Enter the number of pages to view: ")
            : 10;

        var response = await _salesApiService.GetAllSalesForUserAsync(pageNumber, pageSize);

        if (response.IsFailure || response.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{response.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        DisplaySaleResponses(response.Value);
    }

    public async Task HandleViewSaleByIdAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View Sale Details[/]");
        AnsiConsole.MarkupLine("[green]Please choose a sale:[/]");
        await HandleViewAllSalesAsync();

        var saleId = AnsiConsole.Ask<int>("Enter the Id of the sale to view: ");

        var saleResponseResult = await _salesApiService.GetSaleByIdAsync(saleId);

        if (saleResponseResult.IsFailure || saleResponseResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{saleResponseResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        DisplaySaleResponse(saleResponseResult.Value, "Sale Details");
    }

    public async Task HandleViewUserSaleByIdAsync()
    {
        AnsiConsole.MarkupLine("[bold]View Sale Details[/]");
        AnsiConsole.MarkupLine("[green]Please choose a sale:[/]");
        await HandleViewAllSalesForUserAsync();

        var saleId = AnsiConsole.Ask<int>("Enter the Id of the same to view: ");

        var saleResponseResult = await _salesApiService.GetSaleForUserByIdAsync(saleId);

        if (saleResponseResult.IsFailure || saleResponseResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{saleResponseResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        DisplaySaleResponse(saleResponseResult.Value, "Sale Details");
    }

    public async Task HandleCreateSaleAsync()
    {
        AnsiConsole.MarkupLine("[bold]Create a New Sale[/]");
        

        var productsUI = new ProductsUI(_productsApiService, _categoriesApiService);
        
        await productsUI.HandleViewAllProductsAsync();

        var shoppingCart = new List<SaleItemRequest>();

        while (true)
        {
            var productId =
                AnsiConsole.Ask<int>(
                    "Enter a [green]Product Id[/] to add to the cart (or enter [yellow]0[/] to finish shopping:) ");

            if (productId == 0)
            {
                break;
            }

            AnsiConsole.WriteLine("Product details: ");
            await productsUI.ViewProductForSale(productId);
            AnsiConsole.WriteLine();

            var quantity = AnsiConsole.Ask<int>($"Enter the [green]quantity[/] for product {productId}: ");

            if (quantity <= 0)
            {
                AnsiConsole.MarkupLine("[red]Quantity must be a positive number greater than zero.[/]");
                continue;
            }

            shoppingCart.Add(new SaleItemRequest { ProductId = productId, Quantity = quantity });
            AnsiConsole.MarkupLine($"[green]Added Product {productId} (Quantity: {quantity}) to your shopping cart[/]");
        }

        if (shoppingCart.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Sale canceled. No items in cart.[/]");
            return;
        }
        
        AnsiConsole.MarkupLine("[green]Please enter address information:[/]");

        var addressResponse = await GetAddressForSale();
        var streetNumber = addressResponse.StreetNumber;
        var streetName = addressResponse.StreetName;
        var city = addressResponse.City;
        var state = addressResponse.State;
        var country = addressResponse.Country;
        var zipCode = addressResponse.ZipCode;

        var notes = AnsiConsole.Ask<string>("Enter any [green]notes[/] for the sale (optional): ");

        var saleRequest = new CreateSaleRequest
        {
            StreetNumber = streetNumber,
            StreetName = streetName,
            City = city,
            State = state,
            Country = country,
            ZipCode = zipCode,
            Items = shoppingCart,
            Notes = notes
        };

        Result<SaleResponse?> saleResponseResult = null!;

        await AnsiConsole.Status().StartAsync("Placing order...", async _ =>
        {
            saleResponseResult = await _salesApiService.CreateSaleAsync(saleRequest);
        });

        if (saleResponseResult.IsSuccess && saleResponseResult.Value is not null)
        {
            var saleResponse = saleResponseResult.Value;
            AnsiConsole.MarkupLine("[bold green]Sale checkout successful![/]");
            var title = "Receipt";
            DisplaySaleResponse(saleResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{saleResponseResult.Value}[/]");
        }
    }

    public async Task HandleUpdateSaleStatusAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Update sale status[/]");
        AnsiConsole.MarkupLine($"[green]Choose one of the following sales:[/]");
        await HandleViewAllSalesAsync();

        var id = AnsiConsole.Ask<int>("[green]Enter the id of the sale whose status you wish to update: [/]");
        var status = GetSaleStatus();

        var updateSaleStatusRequest = new UpdateSaleStatusRequest
        {
            UpdatedStatus = status
        };

        Result<string?> responseMessageResult = null!;

        await AnsiConsole.Status().StartAsync("Updating sale status...", async _ =>
        {
            responseMessageResult = await _salesApiService.UpdateSaleAsync(id, updateSaleStatusRequest);
        });

        if (responseMessageResult.IsSuccess && responseMessageResult.Value is not null)
        {
            var responseMessage = responseMessageResult.Value;
            AnsiConsole.MarkupLine($"[green]{responseMessage}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{responseMessageResult.ErrorMessage}[/]");
        }
    }

    public async Task HandleRefundSale()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Refund a sale[/]");
        AnsiConsole.MarkupLine("[green]Choose one of the following sales: [/]");
        await HandleViewAllSalesAsync();

        var id = AnsiConsole.Ask<int>("[green]Enter the id of the sale that you wish to refund:[/] ");

        Result<string?> responseMessageResult = null!;

        await AnsiConsole.Status().StartAsync("Refunding sale...", async _ =>
        {
            responseMessageResult = await _salesApiService.RefundSaleAsync(id);
        });

        if (responseMessageResult.IsSuccess && responseMessageResult.Value is not null)
        {
            var responseMessage = responseMessageResult.Value;
            AnsiConsole.MarkupLine($"[green]{responseMessage}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{responseMessageResult.ErrorMessage}[/]");
        }
    }

    public async Task HandleAdminCancelSaleAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Cancel a sale[/]");
        AnsiConsole.MarkupLine("[green]Choose one of the following sales: [/]");
        await HandleViewAllSalesAsync();

        var id = AnsiConsole.Ask<int>("[green]Enter the id of the sale that you wish to cancel:[/]");

        Result<string?> responseMessageResult = null!;

        await AnsiConsole.Status().StartAsync("Canceling sale...", async _ =>
        {
            responseMessageResult = await _salesApiService.CancelSaleAsync(id);
        });

        if (responseMessageResult.IsSuccess && responseMessageResult.Value is not null)
        {
            var responseMessage = responseMessageResult.Value;
            AnsiConsole.MarkupLine($"[green]{responseMessage}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{responseMessageResult.ErrorMessage}[/]");
        }
    }

    public async Task HandleUserCancelSaleAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Cancel a sale[/]");
        AnsiConsole.MarkupLine("[green]Choose one of the following sales: [/]");
        await HandleViewAllSalesForUserAsync();

        var id = AnsiConsole.Ask<int>("[green]Enter the id of the sale that you wish to cancel: [/]");

        Result<string?> responseMessageResult = null!;

        await AnsiConsole.Status().StartAsync("Canceling sale...", async _ =>
        {
            responseMessageResult = await _salesApiService.UserCancelSaleAsync(id);
        });

        if (responseMessageResult.IsSuccess && responseMessageResult.Value is not null)
        {
            var responseMessage = responseMessageResult.Value;
            AnsiConsole.MarkupLine($"[green]{responseMessage}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{responseMessageResult.ErrorMessage}[/]");
        }
    }
    
    private void DisplaySaleResponses(List<SaleResponse> sales)
    {
        var table = new Table()
            .Title("Sales")
            .Border(TableBorder.Rounded);

        table.AddColumn("Sale Id");
        table.AddColumn("Sale Created At");
        table.AddColumn("Sale Updated At");
        table.AddColumn("Customer Id");
        table.AddColumn("Sale Status");
        table.AddColumn("Notes");
        table.AddColumn("Total");

        
        foreach (var sale in sales)
        {
            var updatedAtString = sale.UpdatedAt.HasValue
                ? sale.UpdatedAt.Value.ToString(DateFormat)
                : "N/A";
            
            table.AddRow(
                sale.Id.ToString(),
                sale.CreatedAt.ToString(DateFormat),
                updatedAtString,
                sale.CustomerId,
                $"{sale.Status.GetDisplayName()}",
                sale.Notes ?? "N/A",
                $"${sale.Total:F2}");
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private void DisplaySaleResponse(SaleResponse sale, string title)
    {
        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded);

        table.AddColumn("Sale Id");
        table.AddColumn("Sale Created At");
        table.AddColumn("Sale Updated At");
        table.AddColumn("Customer Id");
        table.AddColumn("Sale Status");
        table.AddColumn("Notes");
        table.AddColumn("Total");

        var saleId = $"{sale.Id}";
        var createdAt = $"{sale.CreatedAt.ToString(DateFormat)}";
        var updatedAtString = sale.UpdatedAt.HasValue
            ? sale.UpdatedAt.Value.ToString(DateFormat)
            : "N/A";
        var customerId = sale.CustomerId;
        var status = $"{sale.Status.GetDisplayName()}";
        var notes = sale.Notes ?? "N/A";
        var total = $"${sale.Total:F2}";

        table.AddRow(
            saleId,
            createdAt,
            updatedAtString,
            customerId,
            status,
            notes,
            total);

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        var saleProducts = sale.SaleItems;
        var productsTitle = $"Sale items for Sale #{saleId}";
        
        DisplaySaleProducts(saleProducts, productsTitle);
    }

    private void DisplaySaleProducts(List<SaleProductResponse> saleProducts, string title)
    {
        if (saleProducts.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no items available[/]");
            AnsiConsole.WriteLine("Press any key to return to previous menu: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded);

        table.AddColumn("Id");
        table.AddColumn("Name");
        table.AddColumn("Quantity In Stock");
        table.AddColumn("Unit Price");
        table.AddColumn("Discount Price");
        table.AddColumn("Gross Price");
        table.AddColumn("Final Price");

        foreach (var product in saleProducts)
        {
            table.AddRow(
                product.ProductId.ToString(),
                product.ProductName,
                $"{product.Quantity}",
                $"${product.UnitPrice:F2}",
                $"${product.DiscountPrice:F2}",
                $"${product.GrossPrice:F2}",
                $"${product.FinalPrice:F2}");
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private SaleStatus GetSaleStatus()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<SaleStatus>()
                .Title("Please choose sale status: ")
                .AddChoices(Enum.GetValues<SaleStatus>())
                .UseConverter(choice => choice.GetDisplayName()));
    }

    private async Task<AddressResponse> GetAddressForSale()
    {
        var addressUI = new AddressUI(_addressApiService);
        
        var useNewAddress = AnsiConsole.Confirm("Do you wish to use a new address for this sale? ");



        AddressResponse? addressResponse;
        
        if (useNewAddress)
        {
            var streetNumber = AnsiConsole.Ask<string>("Enter street number: ");
            var streetName = AnsiConsole.Ask<string>("Enter street name: ");
            var city = AnsiConsole.Ask<string>("Enter city: ");
            var state = AnsiConsole.Ask<string>("Enter state: ");
            var country = AnsiConsole.Ask<string>("Enter country: ");
            var zipCode = AnsiConsole.Ask<string>("Enter zip code: ");

            addressResponse = new AddressResponse
            {
                StreetNumber = streetNumber,
                StreetName = streetName,
                City = city,
                State = state,
                Country = country,
                ZipCode = zipCode
            };
        }
        else
        {
            addressResponse = await addressUI.GetAddress();
        }

        return addressResponse!;

    }
}