using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Sokoban;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SokobanOptions : IOptions
{
    public static string Name => nameof(SokobanOptions);
    
    public const int DefaultBeamWidth = 500;
    public int BeamWidth { get; set; } = DefaultBeamWidth;
}
