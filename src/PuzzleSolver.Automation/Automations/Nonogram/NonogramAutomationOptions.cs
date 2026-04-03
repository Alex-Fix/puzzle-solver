using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Automation.Automations.Nonogram;

public sealed class NonogramAutomationOptions : IOptions
{
    public static string Name => nameof(NonogramAutomationOptions);

    public const int DefaultMoveDelayMs = 1;
    public int MoveDelayMs { get; set; } = DefaultMoveDelayMs;
}
