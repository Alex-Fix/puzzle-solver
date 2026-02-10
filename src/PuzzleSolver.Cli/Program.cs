using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Automation;
using PuzzleSolver.Cli;
using PuzzleSolver.Cli.BallSort;
using PuzzleSolver.Cli.Infrastructure;
using PuzzleSolver.Cli.Interfaces;
using PuzzleSolver.Cli.Sokoban;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core;
using Spectre.Console.Cli;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddInMemoryCollection()
    .Build();

IServiceCollection services = new ServiceCollection()
    .AddCoreServices(configuration)
    .AddAutomationServices(configuration)
    .AddCliServices(configuration);

var registrar = new MicrosoftTypeRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(cfg =>
{
    cfg.SetApplicationName("puzzle-solver");
    cfg.UseAssemblyInformationalVersion();
    cfg.SetApplicationCulture(CultureInfo.InvariantCulture);
    cfg.CancellationExitCode(ExitCodes.Cancelled);
    cfg.SetExceptionHandler((ex, sp) => 
        sp?.Resolve(typeof(IExceptionHandler)) is IExceptionHandler handler ? handler.Handle(ex) : ExitCodes.Unknown);
    cfg.AddCommand<BallSortCommand>("ballsort")
        .WithDescription("Solve a Ball Sort puzzle from given Url");
    cfg.AddCommand<SokobanCommand>("sokoban")
        .WithDescription("Solve a Sokoban puzzle from given Url");
});

return await app.RunAsync(args);

