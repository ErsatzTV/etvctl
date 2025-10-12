using etvctl.Models;
using Spectre.Console;

namespace etvctl.Planning;

public class PlanPrinter
{
    public void Print(PlanModel plan)
    {
        if (plan.IsEmpty())
        {
            AnsiConsole.MarkupLine("No changes. Your infrastructure matches the configuration.");
            return;
        }

        AnsiConsole.MarkupLine("etvctl will perform the following actions:");
        AnsiConsole.MarkupLine("");

        if (plan.SmartCollections.ToAdd.Count != 0)
        {
            foreach (var smartCollection in plan.SmartCollections.ToAdd)
            {
                AnsiConsole.MarkupLine($"  # Smart Collection \"{smartCollection.Name}\" will be created");
                AnsiConsole.MarkupLine($"  [green]+ resource \"smart_collection\" \"{smartCollection.Name}\" {{[/]");
                AnsiConsole.MarkupLine($"  [green]    + name:\t\"{smartCollection.Name}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + query:\t\"{smartCollection.Query}\"[/]");
                AnsiConsole.MarkupLine("  [green]  }[/]");
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
                AnsiConsole.MarkupLine($"  [red]    - query:\t\"{smartCollection.Query}\"[/]");
                AnsiConsole.MarkupLine("  [red]  }[/]");
            }

            AnsiConsole.MarkupLine("");
        }

        AnsiConsole.MarkupLine(
            $"Plan: [green]{plan.ToAdd()} to add[/], [yellow]{plan.ToUpdate()} to change[/], [red]{plan.ToRemove()} to delete[/].");
    }
}
