using System.Runtime.InteropServices;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Sokoban;

public sealed class SokobanState : IState<SokobanState, SokobanMove, SokobanOptions>
{
    [Flags]
    private enum CellType : byte
    {
        Unbound = 1,
        Wall = 2,
        Target = 4,
        Empty = 8,
        Box = 16,
        Player = 32,
        BoxOnTarget = Target | Box,
        PlayerOnTarget = Player | Box
    }

    private readonly int _heigth;
    private readonly int _width;
    private readonly CellType[] _layout;
    
    public SokobanState(int height, int width, string layout)
    {
        _heigth = height;
        _width = width;
        _layout = new CellType[height * width];

        Span<CellType> destinationLayoutSpan = _layout.AsSpan();
        ReadOnlySpan<char> targetLayoutSpan = layout.AsSpan();
        
        for (int i = 0; i < targetLayoutSpan.Length; ++i)
            destinationLayoutSpan[i] = targetLayoutSpan[i] switch
            {
                '!' => CellType.Unbound,
                '#' => CellType.Wall,
                '.' => CellType.Target,
                's' => CellType.Empty,
                '$' => CellType.Box,
                '@' => CellType.Player,
                '*' => CellType.BoxOnTarget,
                '+' => CellType.PlayerOnTarget,
                _ => throw new ArgumentException("Undefined cell type.")
            };
    }

    private SokobanState(int height, int width, CellType[] layout)
    {
        _heigth = height;
        _width = width;
        _layout = layout;
    }
    
    public IEnumerable<SokobanMove> GetValidMoves()
    {
        throw new NotImplementedException();
    }

    public SokobanState Apply(SokobanMove move)
    {
        throw new NotImplementedException();
    }

    public bool IsSolved()
    {
        foreach(CellType cell in _layout.AsSpan())
            if (cell is CellType.Target or CellType.Box or CellType.PlayerOnTarget)
                return false;

        return true;
    }

    public int GetStateHash()
    {
        var hashCode = new HashCode();
        hashCode.AddBytes(MemoryMarshal.AsBytes(_layout.AsSpan()));
        return hashCode.ToHashCode();
    }
    
    public double GetHeuristic(SokobanOptions options)
    {
        double score = 0;
        
        foreach(CellType cell in _layout.AsSpan())
            if (cell is CellType.Target or CellType.Box or CellType.PlayerOnTarget)
                ++score;

        return score;
    }
}
