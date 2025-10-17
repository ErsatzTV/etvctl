using ConsoleAppFramework;
using etvctl.Commands;
using etvctl.Planning;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSingleton<Planner>();
services.AddSingleton<PlanPrinter>();

await using var serviceProvider = services.BuildServiceProvider();
ConsoleApp.ServiceProvider = serviceProvider;

var app = ConsoleApp.Create();

app.Add<ExportCommand>();
app.Add<PlanCommand>();
app.Add<ApplyCommand>();

await app.RunAsync(args);
