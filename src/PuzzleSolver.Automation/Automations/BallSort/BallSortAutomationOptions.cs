using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Automation.Automations.BallSort;

public sealed class BallSortAutomationOptions : IOptions
{
    public static string Name => nameof(BallSortAutomationOptions);

    public const int DefaultMoveDelayMs = 600;
    public int MoveDelayMs { get; set; } = DefaultMoveDelayMs;
}
