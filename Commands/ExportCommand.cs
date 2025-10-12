using ConsoleAppFramework;
using etvctl.Api;
using etvctl.Models;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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

        var rootModel = new RootModel();
        foreach (var smartCollection in smartCollections)
        {
            var model = new SmartCollectionModel { Name = smartCollection.Name, Query = smartCollection.Query };
            rootModel.SmartCollections.Add(model);
        }

        var serializer = new StaticSerializerBuilder(new YamlStaticContext())
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        AnsiConsole.MarkupLine($"Exporting smart collections to {config.Template}");
        await File.WriteAllTextAsync(config.Template!, serializer.Serialize(rootModel), cancellationToken);
    }
}
