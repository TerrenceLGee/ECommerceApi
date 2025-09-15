using Spectre.Console;

namespace ECommerce.Presentation.UI.Helpers;

public static class UIHelper
{
    public static void PrintMessageAndContinue(string errorMessage)
    {
        AnsiConsole.MarkupLine($"[bold red]{errorMessage}[/]");
        AnsiConsole.WriteLine("Press any key to continue: ");
        Console.ReadKey();
        AnsiConsole.Clear();
    }
}