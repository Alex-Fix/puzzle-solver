using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Sokoban;

public sealed class SokobanState : IState<SokobanState, SokobanMove, SokobanOptions>
{
    [Flags]
    private enum CellType : byte
    {
        Empty = 0,
        Wall = 1 << 0,
        Target = 1 << 1,
        Box = 1 << 2,
        Player = 1 << 3,
        Unbound = 1 << 4,
        BoxOnTarget = Box | Target,
        PlayerOnTarget = Player | Target
    }
    
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
        var boxIndices = new List<int>();
        var targetIndices = new List<int>();

        for (int i = 0; i < _layout.Length; i++)
        {
            if (_layout[i].HasFlag(CellType.Box)) boxIndices.Add(i);
            if (_layout[i].HasFlag(CellType.Target)) targetIndices.Add(i);
        }

        double totalManhattanDistance = 0;

        foreach (int boxIdx in boxIndices)
        {
            if (_layout[boxIdx].HasFlag(CellType.Target)) continue;

            if (IsPermanentDeadlock(boxIdx))
                return double.PositiveInfinity;

            int boxX = boxIdx % _width;
            int boxY = boxIdx / _width;
            
            int minDistance = int.MaxValue;
            foreach (int targetIdx in targetIndices)
            {
                int targetX = targetIdx % _width;
                int targetY = targetIdx / _width;
                
                int dist = Math.Abs(boxX - targetX) + Math.Abs(boxY - targetY);
                if (dist < minDistance) minDistance = dist;
            }
            
            totalManhattanDistance += minDistance;
        }

        int playerIdx = FindIndex(_layout, CellType.Player);
        if (boxIndices.Count > 0)
        {
            int pX = playerIdx % _width;
            int pY = playerIdx / _width;
            int minPlayerDist = boxIndices.Min(b => Math.Abs(pX - (b % _width)) + Math.Abs(pY - (b / _width)));
            totalManhattanDistance += (minPlayerDist - 1); 
        }

        return totalManhattanDistance;
    }

    private bool IsPermanentDeadlock(int index)
    {
        bool up = _layout[GetOffset(index, SokobanMoveDirection.Up)].HasFlag(CellType.Wall);
        bool down = _layout[GetOffset(index, SokobanMoveDirection.Down)].HasFlag(CellType.Wall);
        bool left = _layout[GetOffset(index, SokobanMoveDirection.Left)].HasFlag(CellType.Wall);
        bool right = _layout[GetOffset(index, SokobanMoveDirection.Right)].HasFlag(CellType.Wall);

        if ((up && left) || (up && right) || (down && left) || (down && right))
            return true;

        return false;
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
                _   => throw new ArgumentException($"Unknown direction: {direction}")
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
