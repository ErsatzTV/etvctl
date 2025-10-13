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

        var templateModel = new TemplateModel();

        // ffmpeg profiles
        foreach (var ffmpegProfile in await client.GetFFmpegProfiles(cancellationToken))
        {
            var model = new FFmpegProfileModel(ffmpegProfile);
            templateModel.FFmpegProfiles.Add(model);
        }

        // smart collections
        foreach (var smartCollection in await client.GetSmartCollections(cancellationToken))
        {
            var model = new SmartCollectionModel(smartCollection);
            templateModel.SmartCollections.Add(model);
        }

        await YamlWriter.WriteTemplate(config, templateModel, cancellationToken);
    }
}
