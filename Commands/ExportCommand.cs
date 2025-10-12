using ConsoleAppFramework;
using etvctl.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace etvctl.Commands;

public class ExportCommand()
{
    [Command("export")]
    public void Run(string server, CancellationToken cancellationToken = default)
    {
        var model = new RootModel();
        model.Channels.Add(new ChannelModel { Number = "1", Name = "First Channel" });

        var serializer = new StaticSerializerBuilder(new YamlStaticContext())
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        Console.WriteLine(serializer.Serialize(model));
    }
}
