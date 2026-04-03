using Microsoft.Z3;
using PuzzleSolver.Core.Exceptions;

namespace PuzzleSolver.Core.Nonogram.Solvers;

public sealed class SmtSolver : INonogramSolver
{
    public IEnumerable<NonogramMove> Solve(NonogramState initialState, CancellationToken cancellationToken = default)
    {
        using Context ctx = new();
        Solver solver = ctx.MkSolver();

        int rows = initialState.RowClues.Length;
        int cols = initialState.ColumnClues.Length;

        BoolExpr[,] grid = new BoolExpr[rows, cols];
        for (int r = 0; r < rows; ++r)
            for (int c = 0; c < cols; ++c)    
                grid[r, c] = ctx.MkBoolConst($"c_{r}_{c}");

        AddLineConstraints(ctx, solver, initialState.RowClues, (r, i) => grid[r, i], rows, cols, "r");
        AddLineConstraints(ctx, solver, initialState.ColumnClues, (c, i) => grid[i, c], cols, rows, "c");

        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException(nameof(Solve));

        Status status = solver.Check();
        if(status is not Status.SATISFIABLE)
            throw new SolutionNotFoundException();
        
        Model m = solver.Model;
        var solution = new List<NonogramMove>();
        for (int r = 0; r < rows; ++r)
            for (int c = 0; c < cols; ++c)
            {
                bool isFilled = m.Evaluate(grid[r, c]).BoolValue == Z3_lbool.Z3_L_TRUE;
                solution.Add(new NonogramMove(r, c, isFilled));
            }

        return solution;
    }

    private static void AddLineConstraints(Context ctx, Solver solver, byte[][] clues, Func<int, int, BoolExpr> getCell, int numLines, int lineLen, string prefix)
    {
        for (int i = 0; i < numLines; i++)
        {
            var lineClues = clues[i];
            int nBlocks = lineClues.Length;
            if (nBlocks == 0)
            {
                for (int j = 0; j < lineLen; j++)
                    solver.Add(ctx.MkNot(getCell(i, j)));
                
                continue;
            }

            IntExpr[] starts = new IntExpr[nBlocks];
            for (int b = 0; b < nBlocks; b++)
            {
                starts[b] = ctx.MkIntConst($"{prefix}_{i}_b_{b}");
                
                solver.Add(ctx.MkGe(starts[b], ctx.MkInt(0)));
                solver.Add(ctx.MkLe(ctx.MkAdd(starts[b], ctx.MkInt(lineClues[b])), ctx.MkInt(lineLen)));

                if (b <= 0)
                    continue;
                
                solver.Add(ctx.MkGe(starts[b], ctx.MkAdd(starts[b - 1], ctx.MkInt(lineClues[b - 1] + 1))));
            }

            for (int j = 0; j < lineLen; j++)
            {
                BoolExpr[] inAnyBlock = new BoolExpr[nBlocks];
                for (int b = 0; b < nBlocks; b++)
                {
                    inAnyBlock[b] = ctx.MkAnd(
                        ctx.MkGe(ctx.MkInt(j), starts[b]), 
                        ctx.MkLt(ctx.MkInt(j), ctx.MkAdd(starts[b], ctx.MkInt(lineClues[b])))
                    );
                }
                
                solver.Add(ctx.MkEq(getCell(i, j), ctx.MkOr(inAnyBlock)));
            }
        }
    }
}
