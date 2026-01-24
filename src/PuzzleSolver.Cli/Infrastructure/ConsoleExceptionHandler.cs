using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Cli.Interfaces;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core.Exceptions;
using Spectre.Console;

namespace PuzzleSolver.Cli.Infrastructure;

internal sealed class ConsoleExceptionHandler : IExceptionHandler
{
    private readonly IAnsiConsole _console;

    public ConsoleExceptionHandler(IAnsiConsole console)
        => _console = console;
    
    public int Handle(Exception ex)
    {
        return ex switch
        {
            UrlMismatchException urlEx => RenderError("Url Mismatch", urlEx.Message, Color.Yellow, ExitCodes.UrlMismatch),
            InitialStateNotFoundException stateEx => RenderError("Automation Error", stateEx.Message, Color.Orange1, ExitCodes.InitialStateNotFound),
            SolutionNotFoundException solveEx => RenderError("Solver Error", solveEx.Message, Color.Red, ExitCodes.SolutionNotFound),
            OperationCanceledException => RenderError("Cancelled", "The operation was aborted by the user.", Color.Grey, ExitCodes.Cancelled),
            _ => RenderCritical(ex)
        };
    }

    private int RenderError(string title, string message, Color color, int exitCode)
    {
        _console.WriteLine();
        _console.Write(new Panel(message)
        {
            Header = new PanelHeader($" {title} "),
            BorderStyle = new Style(color),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 0, 1, 0)
        });
        
        return exitCode;
    }

    private int RenderCritical(Exception ex)
    {
        _console.Write(new Rule("[red]Unexpected Critical Error[/]") { Justification = Justify.Left });
        _console.WriteException(ex, ExceptionFormats.ShortenEverything);
        return ExitCodes.Unknown;
    }
}
