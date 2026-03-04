using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.BallSort.Solvers;

internal class ParallelAStarSolver : BaseParallelAStarSolver<BallSortState, BallSortMove, BallSortOptions>, IBallSortSolver
{
    public ParallelAStarSolver(IOptionsMonitor<BallSortOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    protected override IEnumerable<BallSortMove> ReconstructPath(SearchNode endNode)
        => base.ReconstructPath(endNode).Concat(endNode.State.GetSortingMoves());
}
