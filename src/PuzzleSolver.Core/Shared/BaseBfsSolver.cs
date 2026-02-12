using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Exceptions;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Shared;

internal abstract class BaseBfsSolver<TState, TMove, TOptions> : BaseSolver<TState, TMove, TOptions>
    where TState : IState<TState, TMove, TOptions>
    where TMove : struct, IMove
    where TOptions : class, IOptions
{
    protected BaseBfsSolver(IOptionsMonitor<TOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    public override IEnumerable<TMove> Solve(TState initialState, CancellationToken cancellationToken = default)
    {
        var visited = new HashSet<int> { initialState.GetStateHash() };
        var frontier = new Queue<SearchNode>();
        
        frontier.Enqueue(new SearchNode(initialState));
        while (frontier.TryDequeue(out SearchNode? node))
        {
            if(cancellationToken.IsCancellationRequested) 
                throw new OperationCanceledException(nameof(Solve));
            
            TState state = node.State;
            foreach (TMove move in state.GetValidMoves())
            {
                TState nextState = state.Apply(move);

                if (nextState.IsSolved())
                    return ReconstructPath(new SearchNode(nextState, move, node));
                    
                if (!visited.Add(nextState.GetStateHash())) 
                    continue;
                
                frontier.Enqueue(new SearchNode(nextState, move, node));
            }
        }

        throw new SolutionNotFoundException();
    }
}
