using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Exceptions;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Shared;

internal abstract class BaseParallelAStarSolver<TState, TMove, TOptions> : BaseSolver<TState, TMove, TOptions>
    where TState : IState<TState, TMove, TOptions>
    where TMove : struct, IMove
    where TOptions : class, IOptions
{
    protected BaseParallelAStarSolver(IOptionsMonitor<TOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    public override IEnumerable<TMove> Solve(TState initialState, CancellationToken cancellationToken = default)
    {
        var visited = new ConcurrentDictionary<int, byte>();
        var frontier = new PriorityQueue<SearchNode, double>();
        
        var synchronization = new Lock();
        SearchNode? solution = null;

        visited.TryAdd(initialState.GetStateHash(), 0);
        frontier.Enqueue(new SearchNode(initialState), initialState.GetHeuristic(_options));
        
        while (frontier.TryDequeue(out SearchNode? node, out _))
        {
            if(cancellationToken.IsCancellationRequested) 
                throw new OperationCanceledException(nameof(Solve));
            
            TState state = node.State;

            Parallel.ForEach(
                state.GetValidMoves(),
                new ParallelOptions { CancellationToken = cancellationToken },
                (move, loopState) =>
                {
                    if (loopState.ShouldExitCurrentIteration)
                        return;

                    TState nextState = state.Apply(move);

                    if (!visited.TryAdd(nextState.GetStateHash(), 0))
                        return;
                    
                    if (nextState.IsSolved())
                    {
                        loopState.Stop();

                        using (synchronization.EnterScope())
                            solution = new SearchNode(nextState, move, node);
                        
                        return;
                    }

                    using (synchronization.EnterScope())
                        frontier.Enqueue(new SearchNode(nextState, move, node), nextState.GetHeuristic(_options));
                });

            if (solution is not null)
                return ReconstructPath(solution);
        }

        throw new SolutionNotFoundException();
    }
}

