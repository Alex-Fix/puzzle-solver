using Microsoft.Extensions.Options;

namespace PuzzleSolver.Core.BallSort.Solvers;

internal abstract class BaseBallSortSolver : IDisposable
{
    protected volatile BallSortOptions _options;
    private readonly IDisposable? _optionsSubscription;
    
    protected BaseBallSortSolver(IOptionsMonitor<BallSortOptions> optionsMonitor)
    {
        _options = optionsMonitor.CurrentValue;
        _optionsSubscription = optionsMonitor.OnChange(value => _options = value);
    }
    
    public void Dispose()
        => _optionsSubscription?.Dispose();
    
    protected static IEnumerable<BallSortMove> ReconstructPath(SearchNode endNode)
    {
        var stack = new Stack<BallSortMove>();
        
        for (SearchNode node = endNode; node.HasMove; node = node.Parent!)
            stack.Push(node.Move);

        return stack;
    }
    
    protected sealed class SearchNode
    {
        // Made fields for higher performance
        public readonly BallSortState State;
        public readonly BallSortMove Move;
        public readonly SearchNode? Parent;
        public readonly bool HasMove;

        public SearchNode(BallSortState state, BallSortMove move, SearchNode parent)
        {
            State = state;
            Move = move;
            Parent = parent;
            HasMove = true;
        }

        public SearchNode(BallSortState state)
        {
            State = state;
            HasMove = false;
        }
    }
}
