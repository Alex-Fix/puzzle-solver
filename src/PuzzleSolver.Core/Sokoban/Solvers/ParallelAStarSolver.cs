using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.Sokoban.Solvers;

internal class ParallelAStarSolver : BaseParallelAStarSolver<SokobanState, SokobanMove, SokobanOptions>, ISokobanSolver
{
    public ParallelAStarSolver(IOptionsMonitor<SokobanOptions> optionsMonitor) : base(optionsMonitor)
    {
    }
}
