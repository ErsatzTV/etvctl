using ConsoleAppFramework;
using etvctl.Api;
using etvctl.Planning;
using etvctl.Models.Config;
using Spectre.Console;

namespace etvctl.Commands;

public class PlanCommand(Planner planner, PlanPrinter planPrinter) : BaseCommand
{
    [Command("plan")]
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
    }
}
