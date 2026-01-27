namespace PuzzleSolver.Automation.Interfaces;

public interface IAutomationFactory : IAsyncDisposable
{
    Task InstallAsync(CancellationToken cancellationToken = default);
    Task<TAutomation> CreateAsync<TAutomation>(CancellationToken cancellationToken = default) where TAutomation : IBaseAutomation;
}
