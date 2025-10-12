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

        ICollection<ChannelResponseModel> channels = await client.Channels(cancellationToken);

        var model = new RootModel();
        foreach (var channel in channels)
        {
            model.Channels.Add(new ChannelModel { Number = channel.Number, Name = channel.Name });
        }

        var serializer = new StaticSerializerBuilder(new YamlStaticContext())
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        Console.WriteLine(serializer.Serialize(model));
    }
}
