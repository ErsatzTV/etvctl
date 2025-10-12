using ConsoleAppFramework;
using etvctl.Api;
using etvctl.Models;
using Refit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace etvctl.Commands;

public class ExportCommand()
{
    [Command("export")]
    public async Task Run(string server, CancellationToken cancellationToken = default)
    {
        var settings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(RefitSerializerContext.Default.Options)
        };

        var client = RestService.For<IErsatzTVv1>(server, settings);
        var channels = await client.Channels();
        
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
