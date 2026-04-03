using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.Nonogram.Solvers;

internal sealed class AStarSolver : BaseAStarSolver<NonogramState, NonogramMove, NonogramOptions>, INonogramSolver
{
    public AStarSolver(IOptionsMonitor<NonogramOptions> optionsMonitor) : base(optionsMonitor)
    {
    }
}
