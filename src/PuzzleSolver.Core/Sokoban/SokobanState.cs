using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Sokoban;

public sealed class SokobanState : IState<SokobanState, SokobanMove, SokobanOptions>
{
    [Flags]
    private enum CellType : byte
    {
        Empty  = 0,
        Wall   = 1 << 0,
        Target = 1 << 1,
        Box    = 1 << 2,
        Player = 1 << 3,
        Unbound = 1 << 4,
        BoxOnTarget = Box | Target,
        PlayerOnTarget = Player | Target
    }
    
    private static IReadOnlyDictionary<SokobanMoveDirection, SokobanMoveDirection> _deadLocked = new Dictionary<SokobanMoveDirection, SokobanMoveDirection>
    {
        [SokobanMoveDirection.Up] = SokobanMoveDirection.Right,
        [SokobanMoveDirection.Right] = SokobanMoveDirection.Down,
        [SokobanMoveDirection.Down] = SokobanMoveDirection.Left,
        [SokobanMoveDirection.Left] = SokobanMoveDirection.Up
    };  
    
    private static IReadOnlyDictionary<SokobanMoveDirection, SokobanMoveDirection> _semiLocked = new Dictionary<SokobanMoveDirection, SokobanMoveDirection>
    {
        [SokobanMoveDirection.Up] = SokobanMoveDirection.Down,
        [SokobanMoveDirection.Right] = SokobanMoveDirection.Left,
        [SokobanMoveDirection.Down] = SokobanMoveDirection.Up,
        [SokobanMoveDirection.Left] = SokobanMoveDirection.Right
    };

    private readonly int _height;
    private readonly int _width;
    private readonly CellType[] _layout;

    public SokobanState(int height, int width, string layout)
    {
        _height = height;
        _width = width;
        _layout = new CellType[height * width];

        for (int i = 0; i < layout.Length; i++)
            _layout[i] = layout[i] switch
            {
                '#' => CellType.Wall,
                '.' => CellType.Target,
                's' => CellType.Empty,
                '$' => CellType.Box,
                '@' => CellType.Player,
                '*' => CellType.BoxOnTarget,
                '+' => CellType.PlayerOnTarget, 
                '!' => CellType.Unbound,
                _   => throw new ArgumentException($"Unknown char: {layout[i]}")
            };
    }

    private SokobanState(int height, int width, CellType[] layout)
    {
        _height = height;
        _width = width;
        _layout = layout;
    }

    public IEnumerable<SokobanMove> GetValidMoves()
    {
        int playerIndex = FindIndex(_layout, CellType.Player);

        for (SokobanMoveDirection direction = SokobanMoveDirection.Start; direction < SokobanMoveDirection.End; ++direction)
        {
            int toIndex = GetOffset(playerIndex, direction);
            if (IsWalkable(_layout, toIndex))
            {
                yield return new SokobanMove(direction);
                continue;
            }

            if (!_layout[toIndex].HasFlag(CellType.Box))
                continue;

            int boxIndex = GetOffset(toIndex, direction);
            if (IsWalkable(_layout, boxIndex))
                yield return new SokobanMove(direction);
        }
    }

    public SokobanState Apply(SokobanMove move)
    {
        var newLayout = (CellType[])_layout.Clone();

        int playerIndex = FindIndex(newLayout, CellType.Player);
        int toIndex = GetOffset(playerIndex, move.Direction);

        if (newLayout[toIndex].HasFlag(CellType.Box))
        {
            int boxIndex = GetOffset(toIndex, move.Direction);
            MoveObject(newLayout, toIndex, boxIndex, CellType.Box);
        }
        
        MoveObject(newLayout, playerIndex, toIndex, CellType.Player);
        
        return new SokobanState(_height, _width, newLayout);
    }

    public bool IsSolved()
    {
        foreach(CellType cell in _layout.AsSpan())
            // All boxes should be on target
            if (cell is CellType.Box)
                return false;

        return true;
    }

    public double GetHeuristic(SokobanOptions options)
    {
        double score = 0;

        int index = 0;
        while (true)
        {
            index = FindIndex(_layout, CellType.Box, index);
            if(index < 0)
                break;

            CellType cell = _layout[index];
            if (cell is CellType.BoxOnTarget)
                continue;

            foreach ((SokobanMoveDirection first, SokobanMoveDirection second) in _deadLocked)
            {
                int firstOffset = GetOffset(index, first);
                int secondOffset = GetOffset(index, second);

                if (!IsWalkable(_layout, firstOffset) && !IsWalkable(_layout, secondOffset))
                {
                    score = double.PositiveInfinity;
                    break;
                }
            }

            foreach ((SokobanMoveDirection first, SokobanMoveDirection second) in _semiLocked)
            {
                int firstOffset = GetOffset(index, first);
                int secondOffset = GetOffset(index, second);

                if (!IsWalkable(_layout, firstOffset) && !IsWalkable(_layout, secondOffset))
                    score += 10;
            }
            
            ++score;
        }

        return score;
    }

    public int GetStateHash()
    {
        var hashCode = new HashCode();
        hashCode.AddBytes(MemoryMarshal.AsBytes(_layout));
        return hashCode.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetOffset(int index, SokobanMoveDirection direction) 
        => direction switch
            {
                SokobanMoveDirection.Up => index - _width,
                SokobanMoveDirection.Right => index + 1,
                SokobanMoveDirection.Down => index + _width,
                SokobanMoveDirection.Left => index - 1,
                _ => throw new ArgumentOutOfRangeException()
            };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindIndex(ReadOnlySpan<CellType> layout, CellType cell, int start = 0)
    {
        for(int index = start; index < layout.Length; ++index)
            if (layout[index].HasFlag(cell))
                return index;

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MoveObject(Span<CellType> layout, int from, int to, CellType cell)
    {
        layout[from] &= ~cell;
        layout[to] |= cell;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsWalkable(ReadOnlySpan<CellType> layout, int index)
        => layout[index] is CellType.Empty or CellType.Target;
}
