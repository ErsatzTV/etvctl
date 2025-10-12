using ConsoleAppFramework;
using etvctl.Api;
using etvctl.Models;
using etvctl.Models.Config;
using etvctl.Planning;
using Spectre.Console;

namespace etvctl.Commands;

public class ExportCommand : BaseCommand
{
    [Command("export")]
    public async Task Run(CancellationToken cancellationToken = default)
    {
        var configAndClient = await ValidateServer(cancellationToken);
        if (configAndClient == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to validate ErsatzTV server[/]");
            return;
        }

        (ConfigModel config, IErsatzTVv1 client) = configAndClient;

        ICollection<SmartCollectionResponseModel> smartCollections =
            await client.GetSmartCollections(cancellationToken);

        var templateModel = new TemplateModel();
        foreach (var smartCollection in smartCollections)
        {
            var model = new SmartCollectionModel { Name = smartCollection.Name, Query = smartCollection.Query };
            templateModel.SmartCollections.Add(model);
        }

        await YamlWriter.WriteTemplate(config, templateModel, cancellationToken);
    }
}
