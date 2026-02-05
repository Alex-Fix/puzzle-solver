using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Exceptions;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.BallSort.Solvers;

internal sealed class BfsSolver : BaseSolver<BallSortState, BallSortMove, BallSortOptions>, IBallSortSolver
{
    public BfsSolver(IOptionsMonitor<BallSortOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    public override IEnumerable<BallSortMove> Solve(BallSortState initialState, CancellationToken cancellationToken = default)
    {
        var visited = new HashSet<int> { initialState.GetStateHash() };
        var frontier = new Queue<SearchNode>();
        
        frontier.Enqueue(new SearchNode(initialState));
        while (frontier.TryDequeue(out SearchNode? node))
        {
            if(cancellationToken.IsCancellationRequested) 
                throw new OperationCanceledException(nameof(Solve));
            
            BallSortState state = node.State;
            foreach (BallSortMove move in state.GetValidMoves())
            {
                BallSortState nextState = state.Apply(move);

                if (nextState.IsSolved())
                    return ReconstructPath(new SearchNode(nextState, move, node)).Concat(nextState.GetSortingMoves());
                    
                if (!visited.Add(nextState.GetStateHash())) 
                    continue;
                
                frontier.Enqueue(new SearchNode(nextState, move, node));
            }
        }

        throw new SolutionNotFoundException();
    }
}
