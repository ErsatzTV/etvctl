using ConsoleAppFramework;
using etvctl.Api;
using etvctl.Models;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace etvctl.Commands;

public class ExportCommand(ILogger<ExportCommand> logger) : BaseCommand(logger)
{
    [Command("export")]
    public async Task Run(CancellationToken cancellationToken = default)
    {
        var configAndClient = await ValidateServer(cancellationToken);
        if (configAndClient == null)
        {
            logger.LogCritical("Failed to validate ErsatzTV server");
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
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        logger.LogInformation("Exporting smart collections to {Template}", config.Template);
        await File.WriteAllTextAsync(config.Template!, serializer.Serialize(rootModel), cancellationToken);
    }
}
