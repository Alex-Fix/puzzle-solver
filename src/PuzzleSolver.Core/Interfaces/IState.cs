namespace PuzzleSolver.Core.Interfaces;

public interface IState<out TState, TMove, in TOptions>
    where TState : IState<TState, TMove, TOptions>
    where TMove : struct, IMove
    where TOptions : class
{
    IEnumerable<TMove> GetValidMoves();
    TState Apply(TMove move);
    bool IsSolved();
    double GetHeuristic(TOptions options);
    int GetStateHash();
}
