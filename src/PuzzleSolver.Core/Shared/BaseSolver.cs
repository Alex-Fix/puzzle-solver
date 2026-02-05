using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Shared;

internal abstract class BaseSolver<TState, TMove, TOptions> : ISolver<TState, TMove, TOptions>, IDisposable
    where TState : IState<TState, TMove, TOptions>
    where TMove : struct, IMove
    where TOptions : class, IOptions
{
    protected volatile TOptions _options;
    private readonly IDisposable? _optionsSubscription;
    
    protected BaseSolver(IOptionsMonitor<TOptions> optionsMonitor)
    {
        _options = optionsMonitor.CurrentValue;
        _optionsSubscription = optionsMonitor.OnChange(value => _options = value);
    }

    public abstract IEnumerable<TMove> Solve(TState initialState, CancellationToken cancellationToken = default);
    
    public void Dispose()
        => _optionsSubscription?.Dispose();
    
    protected static IEnumerable<TMove> ReconstructPath(SearchNode endNode)
    {
        var stack = new Stack<TMove>();
        
        for (SearchNode node = endNode; node.HasMove; node = node.Parent!)
            stack.Push(node.Move);

        return stack;
    }
    
    protected sealed class SearchNode
    {
        // Made fields for higher performance
        public readonly TState State;
        public readonly TMove Move;
        public readonly SearchNode? Parent;
        public readonly bool HasMove;

        public SearchNode(TState state, TMove move, SearchNode parent)
        {
            State = state;
            Move = move;
            Parent = parent;
            HasMove = true;
        }

        public SearchNode(TState state)
        {
            State = state;
            HasMove = false;
        }
    }
}
