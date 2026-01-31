using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.Playwright.Xunit;
using PuzzleSolver.Automation.Automations.BallSort;
using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Core.BallSort;

namespace PuzzleSolver.Automation.IntegrationTests.Automations;

public sealed class BallSortAutomationTests : PageTest, IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture _serviceProviderFixture;

    public BallSortAutomationTests(ServiceProviderFixture serviceProviderFixture)
        => _serviceProviderFixture = serviceProviderFixture;
    
    [Fact]
    public async Task NavigateAsync_WhenWrongUrl()
    {
        const string url = "https://google.com";
        
        IOptionsMonitor<BallSortAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<BallSortAutomationOptions>>();
        using var automation = new BallSortAutomation(Page, optionsMonitor);
        
        await automation
            .Invoking(a => a.NavigateAsync(url))
            .Should()
            .ThrowAsync<UrlMismatchException>();
    }

    [Fact]
    public async Task NavigateAsync_WhenValidUrl()
    {
        const string url = "https://en.grandgames.net/ballsort_classic/id477125";
        
        IOptionsMonitor<BallSortAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<BallSortAutomationOptions>>();
        using var automation = new BallSortAutomation(Page, optionsMonitor);
        
        await automation
            .Invoking(a => a.NavigateAsync(url))
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task ConfigureAsync()
    {
        const string url = "https://en.grandgames.net/ballsort_classic/id477125";
        const string gameContainerSelector = "#main_game_div";
        
        IOptionsMonitor<BallSortAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<BallSortAutomationOptions>>();
        using var automation = new BallSortAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();

        Page.Url.Should().Be(url);
        
        bool isVisible = await Page.IsVisibleAsync(gameContainerSelector);
        isVisible.Should().BeTrue();
    }

    [Fact]
    public async Task GetInitialStateAsync_WhenSort()
    {
        const string url = "https://en.grandgames.net/ballsort_colored/id586145";
        
        IOptionsMonitor<BallSortAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<BallSortAutomationOptions>>();
        using var automation = new BallSortAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();
        
        BallSortState actualState = await automation.GetInitialStateAsync();
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new BallSortState(true, 12, 4, [[],[4,8,4],[4,5,8],[7,1,6],[8,7,2],[2,6,2,2],[2,6,2,6],[3,8,3,1],[0,5,0,7],[0,4,0,3],[5,3,5,7],[1,2,1,2]]);
        int expectedHash = expectedState.GetStateHash();
        
        actualHash.Should().Be(expectedHash);
    }
    
    [Fact]
    public async Task GetInitialStateAsync_WhenNotSort()
    {
        const string url = "https://en.grandgames.net/ballsort_classic/id477125";
        
        IOptionsMonitor<BallSortAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<BallSortAutomationOptions>>();
        using var automation = new BallSortAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();
        
        BallSortState actualState = await automation.GetInitialStateAsync();
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new BallSortState(false, 20, 5, [[],[6,5,5],[5,0,5],[5,6,6,16],[4,1,1,4,1],[1,2,2,1,4],[7,1,1,7,15],[7,8,8,7,2],[15,2,15,4,1],[12,9,9,12,7],[11,14,11,12,8],[8,9,9,8,1],[14,13,13,14,12],[6,0,0,0,3],[10,3,10,10,14],[16,6,16,0,10],[2,15,15,4,1],[16,3,3,16,13],[14,11,11,12,9],[13,3,13,10,11]]);
        int expectedHash = expectedState.GetStateHash();
        
        actualHash.Should().Be(expectedHash);
    }

    [Fact]
    public async Task ApplyMovesAsync()
    {
        const string url = "https://en.grandgames.net/ballsort_classic/id477125";
        const string frameSelector = "#ggPuzzleFrame";
        const string ballSelector = "#tube_0 .ball";
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        List<BallSortMove> moves = [
            new(1, 0),
            new(13, 0)
        ];
        
        IOptionsMonitor<BallSortAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<BallSortAutomationOptions>>();
        using var automation = new BallSortAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();
        await automation.ApplyMovesAsync(moves, cts.Token);

        int ballsCount = await Page.FrameLocator(frameSelector).Locator(ballSelector).CountAsync();
        int expectedBallsCount = 2;

        ballsCount.Should().Be(expectedBallsCount);
    }
}
