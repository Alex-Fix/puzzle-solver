namespace PuzzleSolver.Core.Interfaces;

public interface ISolver<in TState, out TMove, TOptions>
    where TState : IState<TState, TMove, TOptions>
    where TMove : struct, IMove
    where TOptions : class
{
    IEnumerable<TMove> Solve(TState initialState, CancellationToken cancellationToken = default);
}
