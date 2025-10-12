using ConsoleAppFramework;
using etvctl.Api;
using etvctl.Planning;
using etvctl.Models;
using Spectre.Console;

namespace etvctl.Commands;

public class ApplyCommand(Planner planner, PlanPrinter planPrinter) : BaseCommand
{
    [Command("apply")]
    public async Task Run(CancellationToken cancellationToken = default)
    {
        var configAndClient = await ValidateServer(cancellationToken);
        if (configAndClient == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to validate ErsatzTV server[/]");
            return;
        }

        (ConfigModel config, IErsatzTVv1 client) = configAndClient;

        var plan = await planner.DevelopPlan(config, client, cancellationToken);
        planPrinter.Print(plan);

        if (plan.IsEmpty())
        {
            return;
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Markup("Apply this plan? ");

        string? response = Console.ReadLine();
        AnsiConsole.WriteLine();
        if (!string.Equals(response, "yes", StringComparison.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine("[red]Invalid response; aborting[/]");
        }

        if (await planner.ApplyPlan(client, plan, cancellationToken))
        {
            AnsiConsole.MarkupLine("Plan has been applied successfully.");
        }
    }
}
