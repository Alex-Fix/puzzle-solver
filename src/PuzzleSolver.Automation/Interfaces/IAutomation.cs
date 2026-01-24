using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Automation.Interfaces;

public interface IBaseAutomation;

public interface IAutomation<TState, in TMove, TOptions> : IBaseAutomation
    where TState : IState<TState, TMove, TOptions>
    where TMove : struct, IMove
    where TOptions : IOptions
{
    Task NavigateAsync(string url);
    Task ConfigureAsync();
    Task<TState> GetInitialStateAsync();
    Task ApplyMovesAsync(IEnumerable<TMove> moves, CancellationToken cancellationToken = default);
}
