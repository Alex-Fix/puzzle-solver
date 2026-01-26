using PuzzleSolver.Cli.Exceptions;
using PuzzleSolver.Cli.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.Infrastructure;

public sealed class PlaywrightInterceptor : ICommandInterceptor
{
    public void Intercept(CommandContext context, CommandSettings settings)
    {
        // Don't install browser during service command execution
        if (context.Name is "help" or "version") 
            return;
        
        int exitCode = AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Aesthetic)
            .Start("Installing browser dependencies...", _ => Microsoft.Playwright.Program.Main(["install", "chromium"]));

        if (exitCode is ExitCodes.Success)
            return;

        throw new PlaywrightInstallationException();
    }
}
