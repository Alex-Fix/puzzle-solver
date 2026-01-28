namespace PuzzleSolver.Automation.Interfaces;

public interface IAutomationFactory : IAsyncDisposable
{
    Task<TAutomation> CreateAsync<TAutomation>(CancellationToken cancellationToken = default) where TAutomation : IBaseAutomation;
}
