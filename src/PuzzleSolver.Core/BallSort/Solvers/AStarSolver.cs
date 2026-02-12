using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.BallSort.Solvers;

internal sealed class AStarSolver : BaseAStarSolver<BallSortState, BallSortMove, BallSortOptions>, IBallSortSolver
{
    public AStarSolver(IOptionsMonitor<BallSortOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    protected override IEnumerable<BallSortMove> ReconstructPath(SearchNode endNode)
        => base.ReconstructPath(endNode).Concat(endNode.State.GetSortingMoves());
}
