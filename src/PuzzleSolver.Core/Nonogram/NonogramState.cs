using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Buffers;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Nonogram;

public sealed class NonogramState : IState<NonogramState, NonogramMove, NonogramOptions>
{
    private enum CellType : byte
    {
        Unknown, 
        Empty,
        Filled
    }

    private readonly byte[][] _rowClues;
    private readonly byte[][] _columnClues;
    private readonly CellType[] _flatGrid;
    private readonly int _rows;
    private readonly int _cols;

    private static readonly SearchValues<byte> _unknownSearcher = SearchValues.Create([(byte)CellType.Unknown]);

    public NonogramState(byte[][] rowClues, byte[][] columnClues)
    {
        _rowClues = rowClues;
        _columnClues = columnClues;
        _rows = rowClues.Length;
        _cols = columnClues.Length;
        _flatGrid = new CellType[_rows * _cols];
    }

    private NonogramState(byte[][] rowClues, byte[][] columnClues, CellType[] flatGrid, int rows, int cols)
    {
        _rowClues = rowClues;
        _columnClues = columnClues;
        _flatGrid = flatGrid;
        _rows = rows;
        _cols = cols;
    }

    public byte[][] RowClues => _rowClues;
    public byte[][] ColumnClues => _columnClues;
    
    public IEnumerable<NonogramMove> GetValidMoves()
    {
        int idx = MemoryMarshal.AsBytes(_flatGrid.AsSpan()).IndexOfAny(_unknownSearcher);
        if (idx == -1) yield break;

        int r = idx / _cols;
        int c = idx % _cols;

        if (IsPotentiallyValid(r, c, CellType.Filled))
            yield return new NonogramMove(r, c, true);

        if (IsPotentiallyValid(r, c, CellType.Empty))
            yield return new NonogramMove(r, c, false);
    }

    public NonogramState Apply(NonogramMove move)
    {
        CellType[] nextGrid = GC.AllocateUninitializedArray<CellType>(_flatGrid.Length);
        _flatGrid.AsSpan().CopyTo(nextGrid);
        
        nextGrid[move.Row * _cols + move.Column] = move.Filled ? CellType.Filled : CellType.Empty;

        return new NonogramState(_rowClues, _columnClues, nextGrid, _rows, _cols);
    }

    public bool IsSolved()
    {
        if (_flatGrid.AsSpan().Contains(CellType.Unknown)) return false;

        for (int r = 0; r < _rows; r++)
            if (!IsLineValid(GetRowSpan(r), _rowClues[r], true)) return false;

        for (int c = 0; c < _cols; c++)
            if (!IsLineValid(GetColumnSpan(c), _columnClues[c], true)) return false;

        return true;
    }

    public double GetHeuristic(NonogramOptions options) 
        => _flatGrid.AsSpan().Count(CellType.Unknown);

    public int GetStateHash()
    {
        var hash = new HashCode();
        hash.AddBytes(MemoryMarshal.AsBytes(_flatGrid));
        return hash.ToHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySpan<CellType> GetRowSpan(int row) 
        => _flatGrid.AsSpan(row * _cols, _cols);

    private ReadOnlySpan<CellType> GetColumnSpan(int col)
    {
        Span<CellType> colData = stackalloc CellType[_rows];
        for (int r = 0; r < _rows; r++) colData[r] = _flatGrid[r * _cols + col];
        return colData.ToArray(); 
    }

    private bool IsPotentiallyValid(int row, int col, CellType val)
    {
        Span<CellType> rTmp = stackalloc CellType[_cols];
        GetRowSpan(row).CopyTo(rTmp);
        rTmp[col] = val;
        if (!IsLineValid(rTmp, _rowClues[row], false)) return false;

        Span<CellType> cTmp = stackalloc CellType[_rows];
        for (int i = 0; i < _rows; i++) cTmp[i] = _flatGrid[i * _cols + col];
        cTmp[row] = val;
        return IsLineValid(cTmp, _columnClues[col], false);
    }

    private static bool IsLineValid(ReadOnlySpan<CellType> line, byte[] clues, bool mustBeComplete)
    {
        int clueIdx = 0;
        int currentBlockLen = 0;

        for (int i = 0; i < line.Length; i++)
        {
            CellType cell = line[i];

            if (cell == CellType.Filled)
            {
                currentBlockLen++;
                if (clueIdx >= clues.Length || currentBlockLen > clues[clueIdx]) return false;
            }
            else if (cell == CellType.Empty)
            {
                if (currentBlockLen > 0)
                {
                    if (currentBlockLen != clues[clueIdx]) return false;
                    clueIdx++;
                    currentBlockLen = 0;
                }
            }
            else
            {
                if (mustBeComplete) return false;

                int remainingSpace = line.Length - i;
                int minNeeded = 0;

                if (currentBlockLen > 0)
                {
                    minNeeded += (clues[clueIdx] - currentBlockLen);
                    if (clueIdx + 1 < clues.Length) minNeeded += 1;
                    
                    for (int j = clueIdx + 1; j < clues.Length; j++)
                        minNeeded += clues[j] + (j == clues.Length - 1 ? 0 : 1);
                }
                else
                {
                    for (int j = clueIdx; j < clues.Length; j++)
                        minNeeded += clues[j] + (j == clues.Length - 1 ? 0 : 1);
                }

                return minNeeded <= remainingSpace;
            }
        }

        if (currentBlockLen > 0)
        {
            if (clueIdx >= clues.Length || currentBlockLen != clues[clueIdx]) return false;
            clueIdx++;
        }

        return !mustBeComplete || clueIdx == clues.Length;
    }
}
