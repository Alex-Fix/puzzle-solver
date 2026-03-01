using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.Sokoban.Solvers;

internal sealed class AStarSolver : BaseAStarSolver<SokobanState, SokobanMove, SokobanOptions>, ISokobanSolver
{
    public AStarSolver(IOptionsMonitor<SokobanOptions> optionsMonitor) : base(optionsMonitor)
    {
    }
}
