using ConsoleAppFramework;
using etvctl.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger;

var services = new ServiceCollection();

services.AddLogging(x =>
{
    // min level and stderr threshold both warning, so nothing will interfere with stdout
    x.ClearProviders();
    x.SetMinimumLevel(LogLevel.Warning);
    x.AddZLoggerConsole(o => o.LogToStandardErrorThreshold = LogLevel.Warning);
});

await using var serviceProvider = services.BuildServiceProvider();
ConsoleApp.ServiceProvider = serviceProvider;

var app = ConsoleApp.Create();

app.Add<ExportCommand>();

await app.RunAsync(args);
