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
    public async Task Run(string server, CancellationToken cancellationToken = default)
    {
        var client = await ValidateServer(server, cancellationToken);
        if (client == null)
        {
            logger.LogCritical("Failed to validate server {Server}", server);
            return;
        }

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

        Console.WriteLine(serializer.Serialize(rootModel));
    }
}
