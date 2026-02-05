using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Exceptions;
using PuzzleSolver.Core.Shared;

namespace PuzzleSolver.Core.Sokoban.Solvers;

internal sealed class SokobanBeamSearchSolver : BaseSolver<SokobanState, SokobanMove, SokobanOptions>, ISokobanSolver
{
    public SokobanBeamSearchSolver(IOptionsMonitor<SokobanOptions> optionsMonitor) : base(optionsMonitor)
    {
    }

    public override IEnumerable<SokobanMove> Solve(SokobanState initialState, CancellationToken cancellationToken = default)
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
                SokobanState state = node.State;

                foreach (SokobanMove move in state.GetValidMoves())
                {
                    SokobanState nextState = state.Apply(move);

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
}
