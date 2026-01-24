using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PuzzleSolver.Automation.Interfaces;

namespace PuzzleSolver.Automation.AutomationFactories;

internal sealed class PlaywrightAutomationFactory : IAutomationFactory
{
    private readonly SemaphoreSlim _lock = new(1, 1);

    private readonly IServiceProvider _serviceProvider;
    private volatile AutomationFactoryOptions _options;
    private readonly IDisposable? _optionsSubscription;
    
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public PlaywrightAutomationFactory(IServiceProvider serviceProvider, IOptionsMonitor<AutomationFactoryOptions> optionsMonitor)
    {
        _serviceProvider = serviceProvider;
        _options = optionsMonitor.CurrentValue;
        _optionsSubscription = optionsMonitor.OnChange(value => _options = value);
    }
    
    public async Task<TAutomation> CreateAsync<TAutomation>(CancellationToken cancellationToken = default) where TAutomation : IBaseAutomation
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            _playwright ??= await Playwright.CreateAsync();
            _browser ??= await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = _options.Headless });
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
            _optionsSubscription?.Dispose();
            
            if (_browser is not null)
                await _browser.DisposeAsync();
            
            _playwright?.Dispose();
        }
        finally
        {
            _lock.Release();
            _lock.Dispose();
        }
    }
}
