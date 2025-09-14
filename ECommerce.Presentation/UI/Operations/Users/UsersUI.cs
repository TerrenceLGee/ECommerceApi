using Spectre.Console;
using ECommerce.Presentation.Dtos.Address.Response;
using ECommerce.Presentation.Dtos.Auth.Response;

using ECommerce.Presentation.Interfaces;

namespace ECommerce.Presentation.UI.Operations.Users;

public class UsersUI
{
    private readonly IUserApiService _userApiService;
    private readonly IAddressApiService _addressApiService;
    private const string DateFormat = "MM/dd/yyyy";

    public UsersUI(
        IUserApiService userApiService,
        IAddressApiService addressApiService)
    {
        _userApiService = userApiService;
        _addressApiService = addressApiService;
    }


    public async Task<bool> HandleViewAllUsersAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View all users[/]");
        AnsiConsole.WriteLine();

        var countOfUsersResult = await _userApiService.GetCountOfUsersAsync();

        if (countOfUsersResult.IsFailure || countOfUsersResult.Value == 0)
        {
            AnsiConsole.MarkupLine("[red]There are no users available to display[/]");
            AnsiConsole.WriteLine("Press any key to continue: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return false;
        }

        AnsiConsole.MarkupLine($"There are {countOfUsersResult.Value} users available to view: ");

        var pageNumber = AnsiConsole.Confirm("Would you like to specify the page number to view? (default is 1): ")
            ? AnsiConsole.Ask<int>("Enter page number to view ")
            : 1;

        var pageSize = AnsiConsole.Confirm("Would you like to specify the max number of pages? (default is 10): ")
            ? AnsiConsole.Ask<int>("Enter number of pages to view ")
            : 10;

        var response = await _userApiService.GetAllUsersAsync(pageNumber, pageSize);

        if (response.IsFailure || response.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{response.ErrorMessage}[/]");
            AnsiConsole.MarkupLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return false;
        }

        DisplayUsers(response.Value);
        return true;
    }

    public async Task HandleViewUserByIdAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View an individual user[/]");
        AnsiConsole.MarkupLine("[green]Choose from one of the following users:[/]");

        if (!await HandleViewAllUsersAsync())
        {
            return;
        }

        var userId = AnsiConsole.Ask<string>("Enter the id of the user that you wish to view: ");

        var userResponseResult = await _userApiService.GetUserByIdAsync(userId);

        if (userResponseResult.IsFailure || userResponseResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{userResponseResult.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to continue");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        DisplayUser(userResponseResult.Value, "User details");
    }
    
    
    public async Task HandleViewUserAddressesAsync()
    {
        AnsiConsole.MarkupLine("[bold underline yellow]View a user's addresses[/]");
        AnsiConsole.MarkupLine("[green]Choose from one of the following users[/]");

        if (!await HandleViewAllUsersAsync())
        {
            return;
        }

        var userId = AnsiConsole.Ask<string>("Enter the id of the user whose addresses you wish to view: ");

        var userResult = await _userApiService.GetUserByIdAsync(userId);

        if (userResult.IsFailure || userResult.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{userResult.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to continue: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        var userAddressCount = userResult.Value.Addresses.Count;

        if (userAddressCount == 0)
        {
            AnsiConsole.MarkupLine($"[red]There are no addresses associated with user #{userId}");
            AnsiConsole.WriteLine("Press any key to continue");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        AnsiConsole.MarkupLine($"There are {userAddressCount} addresses available to view");

        var pageNumber = AnsiConsole.Confirm("Would you like to specify the page number to view? (default is 1): ")
            ? AnsiConsole.Ask<int>("Enter page number to view ")
            : 1;

        var pageSize = AnsiConsole.Confirm("Would you like to specify the max number of pages? (default is 10): ")
            ? AnsiConsole.Ask<int>("Enter number of pages to view ")
            : 10;

        var response = await _userApiService.GetUserAddressesByIdAsync(userId, pageNumber, pageSize);

        if (response.IsFailure || response.Value is null)
        {
            AnsiConsole.MarkupLine($"[red]{response.ErrorMessage}[/]");
            AnsiConsole.WriteLine("Press any key to return to the previous menu ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }
        
        DisplayUserAddresses(response.Value, $"Addresses for user ${userId}");
    }

    private void DisplayUsers(List<UserResponse> users)
    {
        var table = new Table()
            .Title("Users:")
            .Border(TableBorder.Rounded);

        table.AddColumn("User Id");
        table.AddColumn("First Name");
        table.AddColumn("Last Name");
        table.AddColumn("Email Address");
        table.AddColumn("Birth Date");
        table.AddColumn("Age");

        foreach (var user in users)
        {
            var ageString = $"{CalculateUserAge(user.BirthDate)}";
            table.AddRow(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.EmailAddress,
                user.BirthDate.ToString(DateFormat),
                ageString);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private void DisplayUser(UserResponse user, string title)
    {
        var table = new Table()
            .Title(title)
            .Border(TableBorder.Rounded);

        table.AddColumn("User Id");
        table.AddColumn("First Name");
        table.AddColumn("Last Name");
        table.AddColumn("Email Address");
        table.AddColumn("Birth Date");
        table.AddColumn("Age");

        var id = user.UserId;
        var firstName = user.FirstName;
        var lastName = user.LastName;
        var emailAddress = user.EmailAddress;
        var birthDate = user.BirthDate.ToString(DateFormat);
        var age = $"{CalculateUserAge(user.BirthDate)}";

        table.AddRow(
            id,
            firstName,
            lastName,
            emailAddress,
            birthDate,
            age);

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        var userAddresses = user.Addresses;
        var addressTable = new Table()
            .Title($"Addresses for user #{id}")
            .Border(TableBorder.Rounded);

        addressTable.AddColumn("Address Type:");
        addressTable.AddColumn("Full Address");

        foreach (var address in userAddresses)
        {
            addressTable.AddRow(
                address.AddressType,
                address.FullAddress);
        }

        AnsiConsole.Write(addressTable);
        AnsiConsole.WriteLine();
    }

    private void DisplayUserAddresses(List<AddressResponse> addresses, string title)
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
    private int CalculateUserAge(DateOnly birthDate)
    {
        return birthDate.DayOfYear > DateTime.Now.DayOfYear
            ? DateTime.Now.AddYears(-1).Year - birthDate.Year
            : DateTime.Now.Year - birthDate.Year;
    }
}