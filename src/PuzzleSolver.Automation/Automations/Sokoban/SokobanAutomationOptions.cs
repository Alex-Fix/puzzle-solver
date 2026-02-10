using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Automation.Automations.Sokoban;

public sealed class SokobanAutomationOptions : IOptions
{
    public static string Name => nameof(SokobanAutomationOptions);

    public const int DefaultMoveDelayMs = 600;
    public int MoveDelayMs { get; set; } = DefaultMoveDelayMs;
}
