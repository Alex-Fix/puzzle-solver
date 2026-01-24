using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Automation;
using PuzzleSolver.Cli.Infrastructure;
using PuzzleSolver.Cli.Interfaces;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace PuzzleSolver.Cli.FunctionalTests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CommandAppFixture
{
    public CommandAppFixture()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();
        
        IServiceCollection services = new ServiceCollection()
            .AddCoreServices(configuration)
            .AddAutomationServices(configuration)
            .AddCliServices(configuration);

        var registrar = new MicrosoftTypeRegistrar(services);
        var console = new TestConsole();
        
        App = new CommandAppTester(registrar: registrar, console: console);
        App.Configure(cfg =>
        {
            cfg.CancellationExitCode(ExitCodes.Cancelled);
            cfg.SetExceptionHandler((ex, sp) => 
                sp?.Resolve(typeof(IExceptionHandler)) is IExceptionHandler handler ? handler.Handle(ex) : ExitCodes.Unknown);  
        });
    }
    
    public CommandAppTester App { get; }
}
