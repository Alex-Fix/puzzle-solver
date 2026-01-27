using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Automation.AutomationFactories;
using PuzzleSolver.Automation.Automations.BallSort;
using PuzzleSolver.Automation.Interfaces;
using PuzzleSolver.Cli.Interfaces;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core.BallSort;
using PuzzleSolver.Core.BallSort.Solvers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.BallSort;

public sealed class BallSortCommand : AsyncCommand<BallSortSettings>
{
    private readonly IAnsiConsole _console;
    private readonly IConfigurationUpdater _configurationUpdater;
    private readonly IAutomationFactory _automationFactory;
    private readonly IServiceProvider _serviceProvider;

    public BallSortCommand(
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

    public override async Task<int> ExecuteAsync(CommandContext context, BallSortSettings settings, CancellationToken cancellationToken)
    {
        await _console
            .Status()
            .Spinner(Spinner.Known.Aesthetic)
            .StartAsync("Syncing settings to configuration...", async ctx =>
            {
                _configurationUpdater.Update(
                    new BallSortOptions { BeamWidth = settings.BeamWidth }, 
                    new AutomationFactoryOptions { Headless =  settings.Headless },
                    new BallSortAutomationOptions { MoveDelayMs =  settings.MoveDelayMs });

                ctx.Status("Initializing browser...");
                using BallSortAutomation automation = await _automationFactory.CreateAsync<BallSortAutomation>(cancellationToken);
                
                ctx.Status("Navigating to puzzle Url...");
                await automation.NavigateAsync(settings.Url);
                
                ctx.Status("Configuring puzzle...");
                await automation.ConfigureAsync();
                
                ctx.Status("Extracting puzzle state...");
                BallSortState initialState = await automation.GetInitialStateAsync();
                
                ctx.Status($"Solving using [bold cyan]{settings.Algorithm}[/]...");
                IBallSortSolver solver = _serviceProvider.GetRequiredKeyedService<IBallSortSolver>(settings.Algorithm);
                IEnumerable<BallSortMove> moves = solver.Solve(initialState, cancellationToken);
                
                ctx.Status("Playing back moves in browser...");
                await automation.ApplyMovesAsync(moves, cancellationToken);
            });        
        
        _console.MarkupLine("[bold green]COMPLETE:[/] Puzzle solved successfully!");
        
        return ExitCodes.Success;
    }
}
