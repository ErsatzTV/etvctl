using etvctl.Api;
using Microsoft.Extensions.Logging;
using Refit;

namespace etvctl.Commands;

public abstract class BaseCommand(ILogger logger)
{
    private const int RequiredApiVersion = 1;

    protected async Task<IErsatzTVv1?> ValidateServer(string server, CancellationToken cancellationToken)
    {
        try
        {
            var settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(RefitSerializerContext.Default.Options)
            };

            var client = RestService.For<IErsatzTVv1>(server, settings);

            var version = await client.GetVersion(cancellationToken);

            if (version.ApiVersion != RequiredApiVersion)
            {
                logger.LogCritical(
                    "Server API version {Version} is not required version {RequiredVersion}",
                    version.ApiVersion,
                    RequiredApiVersion);
                return null;
            }

            return client;
        }
        catch (Exception ex)
        {
            logger.LogCritical("{Message}", ex.Message);
            return null;
        }
    }
}
