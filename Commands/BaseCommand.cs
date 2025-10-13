using System.Text.Json;
using System.Text.Json.Serialization;
using etvctl.Api;
using etvctl.Models.Config;
using Refit;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace etvctl.Commands;

public abstract class BaseCommand
{
    private const int RequiredApiVersion = 2;

    protected async Task<ConfigAndClient?> ValidateServer(CancellationToken cancellationToken)
    {
        try
        {
            if (!File.Exists("config.yml"))
            {
                AnsiConsole.MarkupLine("[red]config.yml is required in the current directory[/]");
                return null;
            }

            var deserializer = new StaticDeserializerBuilder(new YamlStaticContext())
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .WithTypeConverter(new FileOrganizationTypeConverter())
                .Build();

            var config = deserializer.Deserialize<ConfigModel>(
                await File.ReadAllTextAsync("config.yml", cancellationToken));

            if (string.IsNullOrWhiteSpace(config.Server))
            {
                AnsiConsole.MarkupLine("[red]config.yml requires a server[/]");
                return null;
            }

            if (string.IsNullOrWhiteSpace(config.Template) || !Directory.Exists(config.Template))
            {
                AnsiConsole.MarkupLine("[red]config.yml requires a template (directory)[/]");
                return null;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter<HardwareAccelerationKind>(),
                    new JsonStringEnumConverter<VaapiDriver>(),
                    new JsonStringEnumConverter<ScalingBehavior>(),
                    new JsonStringEnumConverter<FFmpegProfileVideoFormat>(),
                    new JsonStringEnumConverter<FFmpegProfileBitDepth>(),
                    new JsonStringEnumConverter<FFmpegProfileTonemapAlgorithm>(),
                    new JsonStringEnumConverter<FFmpegProfileAudioFormat>(),
                    new JsonStringEnumConverter<NormalizeLoudnessMode>(),
                },

                TypeInfoResolver = RefitSerializerContext.Default
            };

            var settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(jsonOptions)
            };

            var client = RestService.For<IErsatzTVv1>(config.Server, settings);

            var version = await client.GetVersion(cancellationToken);

            if (version.ApiVersion != RequiredApiVersion)
            {
                AnsiConsole.MarkupLine($"[red]Server API version {version.ApiVersion} is not required version {RequiredApiVersion}[/]");
                return null;
            }

            return new ConfigAndClient(config, client);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
            return null;
        }
    }

    protected sealed record ConfigAndClient(ConfigModel ConfigModel, IErsatzTVv1 Client);
}
