using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Automation.AutomationFactories;
using PuzzleSolver.Automation.Automations.Sokoban;
using PuzzleSolver.Automation.Interfaces;
using PuzzleSolver.Cli.Interfaces;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core.Sokoban;
using PuzzleSolver.Core.Sokoban.Solvers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.Sokoban;

public sealed class SokobanCommand : AsyncCommand<SokobanSettings>
{
    private readonly IAnsiConsole _console;
    private readonly IConfigurationUpdater _configurationUpdater;
    private readonly IAutomationFactory _automationFactory;
    private readonly IServiceProvider _serviceProvider;

    public SokobanCommand(
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

    public override async Task<int> ExecuteAsync(CommandContext context, SokobanSettings settings, CancellationToken cancellationToken)
    {
        await _console
            .Status()
            .Spinner(Spinner.Known.Aesthetic)
            .StartAsync("Syncing settings to configuration...", async ctx =>
            {
                _configurationUpdater.Update( 
                    new AutomationFactoryOptions { Headless = settings.Headless },
                    new SokobanAutomationOptions { MoveDelayMs = settings.MoveDelayMs });

                ctx.Status("Initializing browser...");
                using SokobanAutomation automation = await _automationFactory.CreateAsync<SokobanAutomation>(cancellationToken);
                
                ctx.Status("Navigating to puzzle Url...");
                await automation.NavigateAsync(settings.Url);
                
                ctx.Status("Configuring puzzle...");
                await automation.ConfigureAsync();
                
                ctx.Status("Extracting puzzle state...");
                SokobanState initialState = await automation.GetInitialStateAsync();
                
                ctx.Status($"Solving using [bold cyan]{settings.Algorithm}[/]...");
                ISokobanSolver solver = _serviceProvider.GetRequiredKeyedService<ISokobanSolver>(settings.Algorithm);
                IEnumerable<SokobanMove> moves = solver.Solve(initialState, cancellationToken);
                
                ctx.Status("Playing back moves in browser...");
                await automation.ApplyMovesAsync(moves, cancellationToken);
            });        
        
        _console.MarkupLine("[bold green]COMPLETE:[/] Puzzle solved successfully!");
        
        return ExitCodes.Success;
    }
}

