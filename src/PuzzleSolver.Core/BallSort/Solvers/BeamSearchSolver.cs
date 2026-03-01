using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Exceptions;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.BallSort.Solvers;

internal sealed class BeamSearchSolver : BaseSolver<BallSortState, BallSortMove, BallSortOptions>, IBallSortSolver
{
    public BeamSearchSolver(IOptionsMonitor<BallSortOptions> optionsMonitor) : base(optionsMonitor)
    {
    }
    
    public override IEnumerable<BallSortMove> Solve(BallSortState initialState, CancellationToken cancellationToken = default) 
    {
        var visited = new HashSet<int> { initialState.GetStateHash() };
        var frontier = new List<SearchNode>(_options.BeamWidth) { new(initialState) };
        var nextCandidates = new PriorityQueue<SearchNode, double>();
        
        while (frontier.Count > 0)
        {
            if(cancellationToken.IsCancellationRequested) 
                throw new OperationCanceledException(nameof(Solve));
            
            nextCandidates.Clear();
            foreach (var node in frontier)
            {
                BallSortState state = node.State;

                foreach (BallSortMove move in state.GetValidMoves())
                {
                    BallSortState nextState = state.Apply(move);

                    if (nextState.IsSolved())
                        return ReconstructPath(new SearchNode(nextState, move, node));
                        
                    if (!visited.Add(nextState.GetStateHash())) 
                        continue;
                    
                    nextCandidates.Enqueue(new SearchNode(nextState, move, node), nextState.GetHeuristic(_options));
                }
            }
            
            frontier.Clear();
            while (frontier.Count < _options.BeamWidth && nextCandidates.TryDequeue(out SearchNode? best, out _))
                frontier.Add(best);
        }

        throw new SolutionNotFoundException();
    }

    protected override IEnumerable<BallSortMove> ReconstructPath(SearchNode endNode)
        => base.ReconstructPath(endNode).Concat(endNode.State.GetSortingMoves());
}
