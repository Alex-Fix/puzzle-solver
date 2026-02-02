using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Exceptions;

namespace PuzzleSolver.Core.BallSort.Solvers;

internal sealed class DfsBallSortSolver : BaseBallSortSolver, IBallSortSolver
{
    public DfsBallSortSolver(IOptionsMonitor<BallSortOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    public IEnumerable<BallSortMove> Solve(BallSortState initialState, CancellationToken cancellationToken = default)
    {
        var visited = new HashSet<int> { initialState.GetStateHash() };
        var frontier = new Stack<SearchNode>();
        
        frontier.Push(new SearchNode(initialState));
        while (frontier.TryPop(out SearchNode? node))
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
                
                frontier.Push(new SearchNode(nextState, move, node));
            }
        }

        throw new SolutionNotFoundException();
    }
}
