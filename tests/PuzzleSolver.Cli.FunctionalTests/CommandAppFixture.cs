using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Automation;
using PuzzleSolver.Automation.Interfaces;
using PuzzleSolver.Cli.Infrastructure;
using PuzzleSolver.Cli.Interfaces;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace PuzzleSolver.Cli.FunctionalTests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class CommandAppFixture : IAsyncLifetime
{
    public CommandAppTester App { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
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

        ITypeResolver resolver = registrar.Build();
        IAutomationFactory factory = resolver.Resolve(typeof(IAutomationFactory)) as IAutomationFactory 
            ?? throw new NullReferenceException(nameof(IAutomationFactory));

        await factory.InstallAsync(cts.Token);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
