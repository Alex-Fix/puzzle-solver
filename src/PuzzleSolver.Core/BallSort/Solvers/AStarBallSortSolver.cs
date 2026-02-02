using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Exceptions;

namespace PuzzleSolver.Core.BallSort.Solvers;

internal sealed class AStarBallSortSolver : BaseBallSortSolver, IBallSortSolver
{
    public AStarBallSortSolver(IOptionsMonitor<BallSortOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    public IEnumerable<BallSortMove> Solve(BallSortState initialState, CancellationToken cancellationToken = default)
    {
        var visited = new HashSet<int> { initialState.GetStateHash() };
        var frontier = new PriorityQueue<SearchNode, double>();
        
        frontier.Enqueue(new SearchNode(initialState), initialState.GetHeuristic(_options));
        while (frontier.TryDequeue(out SearchNode? node, out _))
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
                
                frontier.Enqueue(new SearchNode(nextState, move, node), nextState.GetHeuristic(_options));
            }
        }

        throw new SolutionNotFoundException();
    }
}
