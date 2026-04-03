using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Automation.AutomationFactories;
using PuzzleSolver.Automation.Automations.Nonogram;
using PuzzleSolver.Automation.Interfaces;
using PuzzleSolver.Cli.Interfaces;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core.Nonogram;
using PuzzleSolver.Core.Nonogram.Solvers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.Nonogram;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class NonogramCommand : AsyncCommand<NonogramSettings>
{
    private readonly IAnsiConsole _console;
    private readonly IConfigurationUpdater _configurationUpdater;
    private readonly IAutomationFactory _automationFactory;
    private readonly IServiceProvider _serviceProvider;

    public NonogramCommand(
        IAnsiConsole console,
        IConfigurationUpdater configurationUpdater, 
        IAutomationFactory automationFactory, 
        IServiceProvider serviceProvider)
    {
        _console = console;
        _configurationUpdater = configurationUpdater;
        _automationFactory = automationFactory;
        _serviceProvider = serviceProvider;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, NonogramSettings settings, CancellationToken cancellationToken)
    {
        await _console
            .Status()
            .Spinner(Spinner.Known.Aesthetic)
            .StartAsync("Syncing settings to configuration...", async ctx =>
            {
                _configurationUpdater.Update(
                    new NonogramOptions(), 
                    new AutomationFactoryOptions { Headless = settings.Headless },
                    new NonogramAutomationOptions { MoveDelayMs = settings.MoveDelayMs });

                ctx.Status("Initializing browser...");
                using NonogramAutomation automation = await _automationFactory.CreateAsync<NonogramAutomation>(cancellationToken);
                
                ctx.Status("Navigating to puzzle Url...");
                await automation.NavigateAsync(settings.Url);
                
                ctx.Status("Configuring puzzle...");
                await automation.ConfigureAsync();
                
                ctx.Status("Extracting puzzle state...");
                NonogramState initialState = await automation.GetInitialStateAsync();
                
                ctx.Status($"Solving using [bold cyan]{settings.Algorithm}[/]...");
                INonogramSolver solver = _serviceProvider.GetRequiredKeyedService<INonogramSolver>(settings.Algorithm);
                IEnumerable<NonogramMove> moves = solver.Solve(initialState, cancellationToken);
                
                ctx.Status("Playing back moves in browser...");
                await automation.ApplyMovesAsync(moves, cancellationToken);
            });        
        
        _console.MarkupLine("[bold green]COMPLETE:[/] Puzzle solved successfully!");
        
        return ExitCodes.Success;
    }
}
