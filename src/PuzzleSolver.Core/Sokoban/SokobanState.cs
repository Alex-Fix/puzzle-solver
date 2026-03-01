using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Sokoban;

public sealed class SokobanState : IState<SokobanState, SokobanMove, SokobanOptions>
{
    private const int Deadlock = int.MaxValue;
    
    [Flags]
    private enum CellType : byte
    {
        Empty          = 0,
        Wall           = 1,
        Target         = 2,
        Box            = 4,
        Player         = 8,
        Unbound        = 16,
        BoxOnTarget    = Box    | Target,
        PlayerOnTarget = Player | Target
    }
    
    private readonly int _height;
    private readonly int _width;
    private readonly CellType[] _layout;
    private readonly int _playerIndex;
    private readonly int[] _targetIndices;
    private readonly int[] _targetDistances;

    // Executed once. Does not need optimization
    public SokobanState(int height, int width, string layout)
    {
        int layoutLength = height * width;
        if(layout.Length != layoutLength)
            throw new ArgumentException($"Invalid layout length: {layout.Length}", nameof(layout));
        
        _height = height;
        _width = width;
        _layout = new CellType[layoutLength];

        var targetIndices = new List<int>(layoutLength);
        for (int index = 0; index < layoutLength; ++index)
        {
            _layout[index] = layout[index] switch
            {
                '#' => CellType.Wall,
                '.' => CellType.Target,
                's' => CellType.Empty,
                '$' => CellType.Box,
                '@' => CellType.Player,
                '*' => CellType.BoxOnTarget,
                '+' => CellType.PlayerOnTarget, 
                '!' => CellType.Unbound,
                _   => throw new ArgumentException($"Unknown char: {layout[index]}", nameof(layout))
            };

            if (HasFlag(_layout[index], CellType.Player))
                _playerIndex = index;
            
            if (HasFlag(_layout[index], CellType.Target))
                targetIndices.Add(index);
        }

        _targetIndices = targetIndices.ToArray();
        _targetDistances = new int[_targetIndices.Length * layoutLength];
        
        Array.Fill(_targetDistances, Deadlock);
        // BFS pre-calculation of minimal distances between each cell and each target
        for (int target = 0; target < _targetIndices.Length; ++target)
        {
            // nullify distance to current target
            int targetIndex = _targetIndices[target];
            int distanceOffset = GetDistanceOffset(target, targetIndex, SokobanMoveDirection.None);
            _targetDistances[distanceOffset] = 0;
            
            var queue = new Queue<int>();
            queue.Enqueue(targetIndex);
            
            while (queue.Count > 0)
            {
                int currentIndex = queue.Dequeue();
                int currentDistanceOffset = GetDistanceOffset(target, currentIndex, SokobanMoveDirection.None);
                int neighborDistance = _targetDistances[currentDistanceOffset] == Deadlock ? Deadlock : _targetDistances[currentDistanceOffset] + 1;

                for (SokobanMoveDirection direction = SokobanMoveDirection.Start; direction < SokobanMoveDirection.End; ++direction)
                {
                    int neighborIndex = GetLayoutOffset(currentIndex, direction);
                    if (_layout[neighborIndex] is CellType.Wall or CellType.Unbound)
                        continue;

                    int neighborDistanceOffset = GetDistanceOffset(target, currentIndex, direction);
                    if (_targetDistances[neighborDistanceOffset] <= neighborDistance)
                        continue;

                    _targetDistances[neighborDistanceOffset] = neighborDistance;
                    queue.Enqueue(neighborIndex);
                }
            }
        }
    }

    private SokobanState(int height, int width, CellType[] layout, int playerIndex, int[] targetIndices, int[] targetDistances)
    {
        _height = height;
        _width = width;
        _layout = layout;
        _playerIndex = playerIndex;
        _targetIndices = targetIndices;
        _targetDistances = targetDistances;
    }

    public IEnumerable<SokobanMove> GetValidMoves()
    {
        for (SokobanMoveDirection direction = SokobanMoveDirection.Start; direction < SokobanMoveDirection.End; ++direction)
        {
            int toIndex = GetLayoutOffset(_playerIndex, direction);
            if (IsWalkable(_layout, toIndex))
            {
                yield return new SokobanMove(direction);
                continue;
            }

            if (!HasFlag(_layout[toIndex], CellType.Box))
                continue;

            int boxIndex = GetLayoutOffset(toIndex, direction);
            if (IsWalkable(_layout, boxIndex))
                yield return new SokobanMove(direction);
        }
    }

    public SokobanState Apply(SokobanMove move)
    {
        CellType[] newLayout = (CellType[])_layout.Clone();

        int toIndex = GetLayoutOffset(_playerIndex, move.Direction);
        if (HasFlag(newLayout[toIndex], CellType.Box))
        {
            int boxIndex = GetLayoutOffset(toIndex, move.Direction);
            MoveObject(newLayout, toIndex, boxIndex, CellType.Box);
        }
        
        MoveObject(newLayout, _playerIndex, toIndex, CellType.Player);
        
        return new SokobanState(_height, _width, newLayout, toIndex, _targetIndices, _targetDistances);
    }

    public bool IsSolved()
    {
        foreach(ref int index in _targetIndices.AsSpan())
            if (_layout[index] is not CellType.BoxOnTarget)
                return false;
        
        return true;
    }
    
    public double GetHeuristic(SokobanOptions options)
    {
        double totalHeuristic = 0;
        int boxCount = 0;
        int minPlayerToBox = Deadlock;

        for (int index = 0; index < _layout.Length; ++index)
        {
            if (!HasFlag(_layout[index], CellType.Box))
                continue;
        
            boxCount++;
            
            if (HasFlag(_layout[index], CellType.Target)) 
                continue;
            
            if (IsPermanentDeadlock(index)) 
                return double.PositiveInfinity;
            
            int minTargetDistance = Deadlock;
            for (int target = 0; target < _targetIndices.Length; ++target)
            {
                int distanceOffset = GetDistanceOffset(target, index, SokobanMoveDirection.None);
                int distance = _targetDistances[distanceOffset];
                if (distance < minTargetDistance) 
                    minTargetDistance = distance;
            }

            if (minTargetDistance == Deadlock) 
                return double.PositiveInfinity;

            totalHeuristic += minTargetDistance;

            int playerDistance = GetManhattanDistance(_playerIndex, index);
            if (playerDistance < minPlayerToBox) 
                minPlayerToBox = playerDistance;
        }

        if (boxCount > 0 && minPlayerToBox != Deadlock)
            totalHeuristic += minPlayerToBox - 1;

        return totalHeuristic;
    }
    
    public int GetStateHash()
    {
        var hashCode = new HashCode();
        hashCode.AddBytes(MemoryMarshal.AsBytes(_layout));
        return hashCode.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // Optimized Enum.HasFlag version
    private bool HasFlag(CellType cell, CellType flag)
        => (cell & flag) == flag;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetLayoutOffset(int index, SokobanMoveDirection direction) 
        => direction switch
            {
                SokobanMoveDirection.None => index,
                SokobanMoveDirection.Up => index - _width,
                SokobanMoveDirection.Right => index + 1,
                SokobanMoveDirection.Down => index + _width,
                SokobanMoveDirection.Left => index - 1,
                _   => throw new ArgumentException($"Unknown direction: {direction}")
            };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetDistanceOffset(int target, int index, SokobanMoveDirection direction)
        => target * _layout.Length + GetLayoutOffset(index, direction);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MoveObject(Span<CellType> layout, int from, int to, CellType cell)
    {
        layout[from] &= ~cell;
        layout[to] |= cell;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsWalkable(ReadOnlySpan<CellType> layout, int index)
        => layout[index] is CellType.Empty or CellType.Target;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsPermanentDeadlock(int index)
    {
        int up = GetLayoutOffset(index, SokobanMoveDirection.Up);
        int right = GetLayoutOffset(index, SokobanMoveDirection.Right);
        int down = GetLayoutOffset(index, SokobanMoveDirection.Down);
        int left = GetLayoutOffset(index, SokobanMoveDirection.Left);
        
        bool upBlocked = _layout[up] is CellType.Wall;
        bool rightBlocked = _layout[right] is CellType.Wall;
        bool downBlocked = _layout[down] is CellType.Wall;
        bool leftBlocked = _layout[left] is CellType.Wall;
        
        return upBlocked && rightBlocked || 
               rightBlocked && downBlocked || 
               downBlocked && leftBlocked || 
               leftBlocked && upBlocked;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetManhattanDistance(int indexFrom, int indexTo) 
        => Math.Abs((indexFrom % _width) - (indexTo % _width)) + Math.Abs((indexFrom / _width) - (indexTo / _width));
}
