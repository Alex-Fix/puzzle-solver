using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.BallSort.Solvers;

internal sealed class BfsSolver : BaseBfsSolver<BallSortState, BallSortMove, BallSortOptions>, IBallSortSolver
{
    public BfsSolver(IOptionsMonitor<BallSortOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    protected override IEnumerable<BallSortMove> ReconstructPath(SearchNode endNode)
        => base.ReconstructPath(endNode).Concat(endNode.State.GetSortingMoves());
}
