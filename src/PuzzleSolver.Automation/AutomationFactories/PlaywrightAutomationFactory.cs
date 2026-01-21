using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using PuzzleSolver.Automation.Interfaces;

namespace PuzzleSolver.Automation.AutomationFactories;

internal sealed class PlaywrightAutomationFactory : IAutomationFactory
{
    private readonly SemaphoreSlim _lock = new(1, 1);

    private readonly IServiceProvider _serviceProvider;
    
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public PlaywrightAutomationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<TAutomation> CreateAsync<TAutomation>(CancellationToken cancellationToken = default) where TAutomation : IBaseAutomation
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            _playwright ??= await Playwright.CreateAsync();
            _browser ??= await _playwright.Chromium.LaunchAsync();
        }
        finally
        {
            _lock.Release();
        }

        IBrowserContext context = await _browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        return ActivatorUtilities.CreateInstance<TAutomation>(_serviceProvider, page);
    }
    
    public async ValueTask DisposeAsync()
    {
        await _lock.WaitAsync();

        try
        {
            if (_browser is not null)
                await _browser.DisposeAsync();
            
            _playwright?.Dispose();
        }
        finally
        {
            _lock.Release();
        }
    }
}
