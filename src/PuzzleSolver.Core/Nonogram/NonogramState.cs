using System.Runtime.CompilerServices;
using PuzzleSolver.Core.Interfaces;
// ReSharper disable HeapView.ObjectAllocation.Evident

namespace PuzzleSolver.Core.Nonogram;

public sealed class NonogramState : IState<NonogramState, NonogramMove, NonogramOptions>
{
    private const byte Unknown = 0;
    private const byte Empty = 1;
    private const byte Filled = 2;

    private readonly int _width;
    private readonly int _height;
    private readonly int[][] _rowClues;
    private readonly int[][] _columnClues;
    private readonly byte[] _grid;

    public NonogramState(int width, int height, int[][] rowClues, int[][] columnClues)
    {
        _width = width;
        _height = height;
        _rowClues = rowClues;
        _columnClues = columnClues;
        _grid = new byte[width * height];
    }

    private NonogramState(int width, int height, int[][] rowClues, int[][] columnClues, byte[] grid)
    {
        _width = width;
        _height = height;
        _rowClues = rowClues;
        _columnClues = columnClues;
        _grid = grid;
    }

    public IEnumerable<NonogramMove> GetValidMoves()
    {
        for (int index = 0; index < _grid.Length; ++index)
        {
            if (_grid[index] != Unknown)
                continue;

            int row = index / _width;
            int column = index % _width;

            yield return new NonogramMove(row, column, true);
            yield return new NonogramMove(row, column, false);
            yield break;
        }
    }

    public NonogramState Apply(NonogramMove move)
    {
        byte[] newGrid = (byte[])_grid.Clone();

        int index = move.Row * _width + move.Column;
        newGrid[index] = move.Filled ? Filled : Empty;

        var next = new NonogramState(_width, _height, _rowClues, _columnClues, newGrid);
        next.Propagate();

        return next;
    }

    public bool IsSolved()
    {
        foreach (ref byte cell in _grid.AsSpan())
            if (cell == Unknown)
                return false;

        return true;
    }

    public double GetHeuristic(NonogramOptions options)
    {
        int unknown = 0;

        foreach (ref byte cell in _grid.AsSpan())
            if (cell == Unknown)
                ++unknown;

        return unknown;
    }

    public int GetStateHash()
    {
        var hash = new HashCode();
        hash.AddBytes(_grid.AsSpan());
        return hash.ToHashCode();
    }

    private void Propagate()
    {
        bool changed;

        do
        {
            changed = false;

            for (int row = 0; row < _height; ++row)
                changed |= SolveRow(row);

            for (int column = 0; column < _width; ++column)
                changed |= SolveColumn(column);

        } while (changed);
    }

    private bool SolveRow(int row)
    {
        Span<byte> line = stackalloc byte[_width];

        int offset = row * _width;

        for (int index = 0; index < _width; ++index)
            line[index] = _grid[offset + index];

        bool changed = SolveLine(line, _rowClues[row]);

        for (int index = 0; index < _width; ++index)
            _grid[offset + index] = line[index];

        return changed;
    }

    private bool SolveColumn(int column)
    {
        Span<byte> line = stackalloc byte[_height];

        for (int index = 0; index < _height; ++index)
            line[index] = _grid[index * _width + column];

        bool changed = SolveLine(line, _columnClues[column]);

        for (int index = 0; index < _height; ++index)
            _grid[index * _width + column] = line[index];

        return changed;
    }

    private static bool SolveLine(Span<byte> line, int[] blocks)
    {
        int lineLength = line.Length;

        Span<bool> canFill = stackalloc bool[lineLength];
        Span<bool> canEmpty = stackalloc bool[lineLength];
        Span<byte> temp = stackalloc byte[lineLength];

        bool any = false;
        Enumerate(line, blocks, temp, 0, 0, ref any, canFill, canEmpty);

        bool changed = false;
        for (int index = 0; index < lineLength; ++index)
        {
            if (canFill[index] && !canEmpty[index] && line[index] != Filled)
            {
                line[index] = Filled;
                changed = true;
                continue;
            }

            if (!canFill[index] && canEmpty[index] && line[index] != Empty)
            {
                line[index] = Empty;
                changed = true;
            }
        }

        return changed;
    }

    private static void Enumerate(Span<byte> line, int[] blocks, Span<byte> temp, int position, int blockIndex, ref bool any, Span<bool> canFill, Span<bool> canEmpty)
    {
        int lineLength = line.Length;

        if (blockIndex == blocks.Length)
        {
            for (int index = position; index < lineLength; ++index)
                temp[index] = Empty;

            if (!Consistent(line, temp, lineLength))
                return;

            any = true;
            for (int index = 0; index < lineLength; ++index)
            {
                if (temp[index] == Filled)
                {
                    canFill[index] = true;
                    continue;
                }

                canEmpty[index] = true;
            }

            return;
        }

        int size = blocks[blockIndex];

        for (int start = position; start + size <= lineLength; ++start)
        {
            for (int index = position; index < start; ++index)
                temp[index] = Empty;

            for (int index = 0; index < size; ++index)
                temp[start + index] = Filled;

            int next = start + size;

            if (next < lineLength)
            {
                temp[next] = Empty;
                ++next;
            }

            if (!Consistent(line, temp, next))
                continue;

            Enumerate(line, blocks, temp, next, blockIndex + 1, ref any, canFill, canEmpty);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Consistent(Span<byte> line, Span<byte> candidate, int length)
    {
        for (int index = 0; index < length; ++index)
        {
            byte cell = line[index];
            if (cell != Unknown && cell != candidate[index])
                return false;
        }

        return true;
    }
}
