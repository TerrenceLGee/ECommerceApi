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

    public async Task HandleViewAllAddressesAsync()
    {
        var countOfAddressesResult = await _addressApiService.GetCountOfAddressesAsync();

        if (countOfAddressesResult.IsFailure || countOfAddressesResult.Value == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no addresses available to display[/]");
            AnsiConsole.WriteLine("Press any key to continue: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
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
            return;
        }
        
        DisplayAddresses(response.Value);
    }

    public async Task HandleViewAddressByIdAsync()
    {
        
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