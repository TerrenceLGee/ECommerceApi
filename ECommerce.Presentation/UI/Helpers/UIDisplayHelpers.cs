using Spectre.Console;

namespace ECommerce.Presentation.UI.Helpers;

public static class UIDisplayHelpers
{
    public static void ShowPaginatedItems<T>(List<T> items, string collectionName, Action<List<T>> display,
        int pageSize = 10)
    {
        if (items.Count == 0)
        {
            AnsiConsole.MarkupLine($"[red]There are no {collectionName} available[/]");
            AnsiConsole.WriteLine("Press any key to continue: ");
            Console.ReadKey();
            AnsiConsole.Clear();
            return;
        }

        int pageIndex = 0;
        int pageCount = (int)Math.Ceiling(items.Count / (double)pageSize);

        while (true)
        {
            var pageItems = items
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            display(pageItems);

            AnsiConsole.WriteLine(
                $"[blue]Page {pageIndex + 1} of {pageCount} (showing {pageItems.Count} of {items.Count})[/]");

            var prompt = new SelectionPrompt<string>()
                .Title("Navigate pages:");

            if (pageIndex > 0)
                prompt.AddChoice("Previous");
            prompt.AddChoice("Continue");
            if (pageIndex < pageCount - 1)
                prompt.AddChoice("Next");

            var choice = AnsiConsole.Prompt(prompt);

            if (choice == "Next" && pageIndex < pageCount - 1)
                pageIndex++;
            else if (choice == "Previous" && pageIndex > 0)
                pageIndex--;
            else break;
        }
    }
}