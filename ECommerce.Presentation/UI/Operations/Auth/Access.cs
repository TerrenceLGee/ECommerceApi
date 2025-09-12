using System.Globalization;
using System.Security.Claims;
using ECommerce.Presentation.Dtos.Auth.Request;
using ECommerce.Presentation.Interfaces;
using Spectre.Console;

namespace ECommerce.Presentation.UI.Operations.Auth;

public class Access
{
    private readonly ILoginApiService _loginApiService;
    private const string DateFormat = "mm-dd-yyyy";

    public Access(ILoginApiService loginApiService)
    {
        _loginApiService = loginApiService;
    }

    public async Task HandleRegistration()
    {
        var firstName = AnsiConsole.Ask<string>("[green]Enter your first name: [/]");
        var lastName = AnsiConsole.Ask<string>("[green]Enter your last name: [/]");
        var birthDateString = AnsiConsole.Ask<string>($"[green]Enter your birthdate in format: {DateFormat} [/]");
        DateOnly birthDate;
        while (!DateOnly.TryParseExact(birthDateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
        {
            AnsiConsole.MarkupLine("[red]Invalid birth date entered[/]");
            AnsiConsole.Ask<string>($"[green]Enter your birthdate in format: {DateFormat} [/]");
        }
        var emailAddress = AnsiConsole.Ask<string>("[green]Enter your email address[/]");

        string password;
        while (true)
        {
            password = AnsiConsole.Prompt(new TextPrompt<string>("[green]Enter a [blue]password[/]:[/]").Secret());
            var confirmPassword = AnsiConsole.Prompt(new TextPrompt<string>("[green]Confirm password:[/]").Secret());

            if (password.Equals(confirmPassword))
                break;

            AnsiConsole.MarkupLine("[red]Passwords do not match. Please try again.[/]");
        }

        AnsiConsole.MarkupLine("\n[blue]Address Information:\n[/]");
        var streetNumber = AnsiConsole.Ask<string>("[green]Enter street number: [/]");
        var streetName = AnsiConsole.Ask<string>("[green]Enter street name: [/]");
        var city = AnsiConsole.Ask<string>("[green]Enter city: [/]");
        var state = AnsiConsole.Ask<string>("[green]Enter state: [/]");
        var country = AnsiConsole.Ask<string>("[green]Enter country [/]");
        var addressDescription = AnsiConsole.Ask<string>("[green]Enter address description (i.e. Home, Business etc): [/]");

        var registerRequest = new RegisterRequest
        {
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            Email = emailAddress,
            Password = password,
            ConfirmPassword = password,
            StreetNumber = streetNumber,
            StreetName = streetName,
            City = city,
            State = state,
            Country = country,
            AddressDescription = addressDescription,
        };

        var registered = await _loginApiService.RegisterAsync(registerRequest);

        if (registered.IsSuccess)
        {
            AnsiConsole.MarkupLine($"[bold green]{registered.Value}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold red]Unable to register you at this time...\n{registered.ErrorMessage}[/]");
        }
    }
}