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

        FFmpegProfilePrinter.Print(plan);
        SmartCollectionPrinter.Print(plan);

        AnsiConsole.MarkupLine(
            $"Plan: [green]{plan.ToAdd()} to add[/], [yellow]{plan.ToUpdate()} to change[/], [red]{plan.ToRemove()} to delete[/].");
    }
}
