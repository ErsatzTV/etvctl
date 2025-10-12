using ConsoleAppFramework;
using etvctl.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger;

var services = new ServiceCollection();

services.AddLogging(x =>
{
    x.ClearProviders();
    x.SetMinimumLevel(LogLevel.Information);
    x.AddZLoggerConsole(o => o.LogToStandardErrorThreshold = LogLevel.Warning);
});

await using var serviceProvider = services.BuildServiceProvider();
ConsoleApp.ServiceProvider = serviceProvider;

var app = ConsoleApp.Create();

app.Add<ExportCommand>();

await app.RunAsync(args);
