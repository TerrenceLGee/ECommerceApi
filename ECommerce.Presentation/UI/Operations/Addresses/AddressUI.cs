using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Address.Request;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Interfaces;
using Spectre.Console;

namespace ECommerce.Presentation.UI.Operations.Addresses;

public class AddressUI
{
    private readonly IAddressApiService _addressApiService;

    public AddressUI(IAddressApiService addressApiService)
    {
        _addressApiService = addressApiService;
    }

    public async Task<bool> HandleViewAllAddressesAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View all of your addresses[/]");
        
        var countOfAddressesResult = await _addressApiService.GetCountOfAddressesAsync();

        if (countOfAddressesResult.IsFailure || countOfAddressesResult.Value == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no addresses available to display[/]");
            AnsiConsole.WriteLine("Press any key to continue: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return false;
        }

        AnsiConsole.MarkupLine($"There are {countOfAddressesResult.Value} addresses available to view");

        var pageNumber = AnsiConsole.Confirm("Would you like to specify the page number to view? (default is 1): ")
            ? AnsiConsole.Ask<int>("Enter page number to view ")
            : 1;

        var pageSize = AnsiConsole.Confirm("Would you like to specify the max number of pages? (default is 10): ")
            ? AnsiConsole.Ask<int>("Enter number of pages to view ")
            : 10;

        var response = await _addressApiService.GetAllAddressesAsync(pageNumber, pageSize);

        if (response.IsFailure || response.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{response.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return false;
        }
        
        DisplayAddresses(response.Value);
        return true;
    }

    public async Task HandleViewAddressByIdAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View an individual address[/]");
        AnsiConsole.MarkupLine("[green]Choose from one of the following addresses:[/]");

        if (!await HandleViewAllAddressesAsync())
        {
            return;
        }

        var addressId = AnsiConsole.Ask<int>("Enter the id of the address that you wish to view: ");

        var addressResponseResult = await _addressApiService.GetAddressByIdAsync(addressId);

        if (addressResponseResult.IsFailure || addressResponseResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{addressResponseResult.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to continue");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        DisplayAddress(addressResponseResult.Value, "Address details");

    }

    public async Task HandleAddAddressAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Add an address[/]");
        AnsiConsole.WriteLine();

        var streetNumber = AnsiConsole.Ask<string>("Enter street number: ");
        var streetName = AnsiConsole.Ask<string>("Enter street name: ");
        var city = AnsiConsole.Ask<string>("Enter city: ");
        var state = AnsiConsole.Ask<string>("Enter state: ");
        var country = AnsiConsole.Ask<string>("Enter country: ");
        var zipCode = AnsiConsole.Ask<string>("Enter zip code: ");

        var request = new CreateAddressRequest
        {
            StreetNumber = streetNumber,
            StreetName = streetName,
            City = city,
            State = state,
            Country = country,
            ZipCode = zipCode
        };

        Result<AddressResponse?> addressResponseResult = null!;

        await AnsiConsole.Status().StartAsync("Adding address...", async _ =>
        {
            addressResponseResult = await _addressApiService.AddAddressAsync(request);
        });

        if (addressResponseResult.IsSuccess && addressResponseResult.Value is not null)
        {
            var addressResponse = addressResponseResult.Value;
            AnsiConsole.MarkupLine("[bold green]Address added successfully![/]");
            var title = "Newly added address";
            DisplayAddress(addressResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{addressResponseResult.ErrorMessage}[/]");
        }
    }

    public async Task HandleUpdateAddressAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]Update an address[/]");
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[green]Please choose an address to update: [/]");
        await HandleViewAllAddressesAsync();

        var addressId = AnsiConsole.Ask<int>("Enter the Id of the address to update: ");

        var addressToUpdateResult = await _addressApiService.GetAddressByIdAsync(addressId);

        if (addressToUpdateResult.IsFailure || addressToUpdateResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{addressToUpdateResult.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to continue: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        var addressToUpdate = addressToUpdateResult.Value;

        var streetNumber = AnsiConsole.Confirm("Update street number? ")
            ? AnsiConsole.Ask<string>("Enter updated street number: ")
            : addressToUpdate.StreetNumber;

        var streetName = AnsiConsole.Confirm("Update street name? ")
            ? AnsiConsole.Ask<string>("Enter updated street name: ")
            : addressToUpdate.StreetName;

        var city = AnsiConsole.Confirm("Update city? ")
            ? AnsiConsole.Ask<string>("Enter updated city: ")
            : addressToUpdate.City;

        var state = AnsiConsole.Confirm("Update state? ")
            ? AnsiConsole.Ask<string>("Enter updated state: ")
            : addressToUpdate.State;

        var country = AnsiConsole.Confirm("Update country? ")
            ? AnsiConsole.Ask<string>("Enter updated country: ")
            : addressToUpdate.Country;

        var zipCode = AnsiConsole.Confirm("Update zip code? ")
            ? AnsiConsole.Ask<string>("Enter updated zip code: ")
            : addressToUpdate.ZipCode;

        var request = new UpdateAddressRequest
        {
            StreetNumber = streetNumber,
            StreetName = streetName,
            City = city,
            State = state,
            Country = country,
            ZipCode = zipCode
        };

        Result<AddressResponse?> addressResponseResult = null!;

        await AnsiConsole.Status().StartAsync("Updating address...", async _ =>
        {
            addressResponseResult = await _addressApiService.UpdateAddressAsync(addressId, request);
        });

        if (addressResponseResult.IsSuccess && addressResponseResult.Value is not null)
        {
            var addressResponse = addressResponseResult.Value;
            AnsiConsole.MarkupLine("[bold green]Address updated successfully![/]");
            var title = "Updated address";
            DisplayAddress(addressResponse, title);
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]{addressResponseResult.ErrorMessage}[/]");
        }
    }
    public void DisplayAddresses(List<AddressResponse> addresses)
    {
        var table = new Table()
            .Title("Your addresses")
            .Border(TableBorder.Rounded);

        table.AddColumn("Id");
        table.AddColumn("Street Number");
        table.AddColumn("Street Name");
        table.AddColumn("City");
        table.AddColumn("State");
        table.AddColumn("Country");
        table.AddColumn("Zip Code");
        table.AddColumn("Address Type");
        table.AddColumn("Full Address");

        foreach (var address in addresses)
        {
            table.AddRow(
                address.Id.ToString(),
                address.StreetNumber,
                address.StreetName,
                address.City,
                address.State,
                address.Country,
                address.ZipCode,
                address.AddressType,
                address.FullAddress);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public void DisplayAddress(AddressResponse address, string title)
    {
        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded);

        table.AddColumn("Id");
        table.AddColumn("Street Number");
        table.AddColumn("Street Name");
        table.AddColumn("City");
        table.AddColumn("State");
        table.AddColumn("Country");
        table.AddColumn("Zip Code");
        table.AddColumn("Address Type");
        table.AddColumn("Full Address");

        var id = address.Id.ToString();
        var streetNumber = address.StreetNumber;
        var streetName = address.StreetName;
        var city = address.City;
        var state = address.State;
        var country = address.Country;
        var zipCode = address.Country;
        var AddressType = address.AddressType;
        var fullAddress = address.FullAddress;

        table.AddRow(
            id,
            streetNumber,
            streetName,
            city,
            state,
            country,
            zipCode,
            AddressType,
            fullAddress);

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    public async Task<AddressResponse?> GetAddress()
    {
        var addressesResult = await _addressApiService.GetAddressesForDisplay();

        if (addressesResult.IsFailure || addressesResult.Value is null)
        {
            return null;
        }

        var addresses = addressesResult.Value;
        return AnsiConsole.Prompt(
            new SelectionPrompt<AddressResponse>()
                .Title("Your available addresses")
                .PageSize(10)
                .MoreChoicesText("[grey](If you have more addresses on file move up and down to reveal them)[/]")
                .AddChoices(addresses)
                .UseConverter(a => a.FullAddress));
    }
}