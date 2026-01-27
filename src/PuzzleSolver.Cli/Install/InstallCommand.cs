using PuzzleSolver.Automation.Interfaces;
using PuzzleSolver.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.Install;

public class InstallCommand : AsyncCommand
{
    private readonly IAnsiConsole _console;
    private readonly IAutomationFactory _automationFactory;

    public InstallCommand(IAnsiConsole console, IAutomationFactory automationFactory)
    {
        _console = console;
        _automationFactory = automationFactory;
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        await _console
            .Status()
            .Spinner(Spinner.Known.Aesthetic)
            .StartAsync("Installing browser and dependencies...", async ctx =>
                await _automationFactory.InstallAsync(cancellationToken));

        return ExitCodes.Success;
    }
}
