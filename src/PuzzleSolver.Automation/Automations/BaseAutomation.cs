using Microsoft.Playwright;
using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Automation.Interfaces;
using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Automation.Automations;

public abstract class BaseAutomation<TState, TMove, TOptions> : IAutomation<TState, TMove, TOptions> 
    where TState : IState<TState, TMove, TOptions> 
    where TMove : struct, IMove
    where TOptions : IOptions
{
    private const string ConsentBtnSelector = "button.fc-button.fc-cta-consent.fc-primary-button";
    private const int ConsentBtnTimeoutMs = 10_000;
    
    protected readonly IPage _page;

    protected BaseAutomation(IPage page)
        => _page = page;

    protected abstract string UrlStart { get; }

    public async Task NavigateAsync(string url)
    {
        if (!url.StartsWith(UrlStart))
            throw new UrlMismatchException(UrlStart);
            
        await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
    }

    public virtual async Task ConfigureAsync() 
    {
        try
        {
            await _page.ClickAsync(ConsentBtnSelector, new PageClickOptions { Timeout =  ConsentBtnTimeoutMs });
        }
        catch (TimeoutException)
        {
        }
    }
        
    public abstract Task<TState> GetInitialStateAsync();

    public abstract Task ApplyMovesAsync(IEnumerable<TMove> moves, CancellationToken cancellationToken = default);
}
