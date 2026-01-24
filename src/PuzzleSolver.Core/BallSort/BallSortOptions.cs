using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.BallSort;

public sealed class BallSortOptions : IOptions
{
    public static string Name => nameof(BallSortOptions);
    
    public const int DefaultBeamWidth = 250;
    public int BeamWidth { get; set; } = DefaultBeamWidth;
}
