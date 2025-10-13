using etvctl.Api;
using etvctl.Models;
using Spectre.Console;

namespace etvctl.Planning;

public static class SmartCollectionPrinter
{
    public static void Print(PlanModel plan)
    {
        if (plan.SmartCollections.ToAdd.Count != 0)
        {
            foreach (var smartCollection in plan.SmartCollections.ToAdd)
            {
                AnsiConsole.MarkupLine($"  # Smart Collection \"{smartCollection.Name}\" will be created");
                AnsiConsole.MarkupLine($"  [green]+ resource \"smart_collection\" \"{smartCollection.Name}\" {{[/]");
                AnsiConsole.MarkupLine($"  [green]    + name:\t\"{smartCollection.Name}\"[/]");
                AnsiConsole.MarkupLine("  [green]    + query:\t\"{0}\"[/]", Markup.Escape(smartCollection.Query!));
                AnsiConsole.MarkupLine("  [green]  }[/]");
            }

            AnsiConsole.MarkupLine("");
        }

        if (plan.SmartCollections.ToUpdate.Count != 0)
        {
            foreach ((SmartCollectionModel newValue, SmartCollectionResponseModel oldValue) in plan.SmartCollections
                         .ToUpdate)
            {
                AnsiConsole.MarkupLine($"  # Smart Collection \"{oldValue.Name}\" will be changed");
                AnsiConsole.MarkupLine($"  [yellow]~ resource \"smart_collection\" \"{oldValue.Name}\" {{[/]");

                if (!string.Equals(newValue.Query, oldValue.Query))
                {
                    AnsiConsole.MarkupLine("  [red]    - query:\t\"{0}\"[/]", Markup.Escape(oldValue.Query));
                    AnsiConsole.MarkupLine("  [green]    + query:\t\"{0}\"[/]", Markup.Escape(newValue.Query!));
                }

                if (!string.Equals(newValue.Name, oldValue.Name))
                {
                    AnsiConsole.MarkupLine("  [red]    - name:\t\"{0}\"[/]", Markup.Escape(oldValue.Name));
                    AnsiConsole.MarkupLine("  [green]    + name:\t\"{0}\"[/]", Markup.Escape(newValue.Name!));
                }

                AnsiConsole.MarkupLine("  [yellow]  }[/]");
            }

            AnsiConsole.MarkupLine("");
        }

        if (plan.SmartCollections.ToRemove.Count != 0)
        {
            foreach (var smartCollection in plan.SmartCollections.ToRemove)
            {
                AnsiConsole.MarkupLine($"  # Smart Collection \"{smartCollection.Name}\" will be deleted");
                AnsiConsole.MarkupLine($"  [red]- resource \"smart_collection\" \"{smartCollection.Name}\" {{[/]");
                AnsiConsole.MarkupLine($"  [red]    - name:\t\"{smartCollection.Name}\"[/]");
                AnsiConsole.MarkupLine("  [red]    + query:\t\"{0}\"[/]", Markup.Escape(smartCollection.Query!));
                AnsiConsole.MarkupLine("  [red]  }[/]");
            }

            AnsiConsole.MarkupLine("");
        }
    }
}
