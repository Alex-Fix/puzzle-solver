using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Exceptions;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.Sokoban.Solvers;

internal sealed class AStarSolver : BaseSolver<SokobanState, SokobanMove, SokobanOptions>, ISokobanSolver
{
    public AStarSolver(IOptionsMonitor<SokobanOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    public override IEnumerable<SokobanMove> Solve(SokobanState initialState, CancellationToken cancellationToken = default)
    {
        var visited = new HashSet<int> { initialState.GetStateHash() };
        var frontier = new PriorityQueue<SearchNode, double>();
        
        frontier.Enqueue(new SearchNode(initialState), initialState.GetHeuristic(_options));
        while (frontier.TryDequeue(out SearchNode? node, out _))
        {
            if(cancellationToken.IsCancellationRequested) 
                throw new OperationCanceledException(nameof(Solve));
            
            SokobanState state = node.State;
            foreach (SokobanMove move in state.GetValidMoves())
            {
                SokobanState nextState = state.Apply(move);

                if (nextState.IsSolved())
                    return ReconstructPath(new SearchNode(nextState, move, node));
                    
                if (!visited.Add(nextState.GetStateHash())) 
                    continue;
                
                frontier.Enqueue(new SearchNode(nextState, move, node), nextState.GetHeuristic(_options));
            }
        }

        throw new SolutionNotFoundException();
    }
}
