using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.Playwright.Xunit;
using PuzzleSolver.Automation.Automations.Sokoban;
using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Core.Sokoban;

namespace PuzzleSolver.Automation.IntegrationTests.Automations;

public class SokobanAutomationTests : PageTest, IClassFixture<ServiceProviderFixture>
{
     private readonly ServiceProviderFixture _serviceProviderFixture;

    public SokobanAutomationTests(ServiceProviderFixture serviceProviderFixture)
        => _serviceProviderFixture = serviceProviderFixture;
    
    [Fact]
    public async Task NavigateAsync_WhenWrongUrl()
    {
        const string url = "https://google.com";
        
        IOptionsMonitor<SokobanAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<SokobanAutomationOptions>>();
        using var automation = new SokobanAutomation(Page, optionsMonitor);
        
        await automation
            .Invoking(a => a.NavigateAsync(url))
            .Should()
            .ThrowAsync<UrlMismatchException>();
    }

    [Fact]
    public async Task NavigateAsync_WhenValidUrl()
    {
        const string url = "https://en.grandgames.net/sokoban/id195119";
        
        IOptionsMonitor<SokobanAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<SokobanAutomationOptions>>();
        using var automation = new SokobanAutomation(Page, optionsMonitor);
        
        await automation
            .Invoking(a => a.NavigateAsync(url))
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task ConfigureAsync()
    {
        const string url = "https://en.grandgames.net/sokoban/id195119";
        const string gameContainerSelector = "#main_game_div";
        
        IOptionsMonitor<SokobanAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<SokobanAutomationOptions>>();
        using var automation = new SokobanAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();

        Page.Url.Should().Be(url);
        
        bool isVisible = await Page.IsVisibleAsync(gameContainerSelector);
        isVisible.Should().BeTrue();
    }

    [Fact]
    public async Task GetInitialStateAsync()
    {
        const string url = "https://en.grandgames.net/sokoban/id195119";
        
        IOptionsMonitor<SokobanAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<SokobanAutomationOptions>>();
        using var automation = new SokobanAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();
        
        SokobanState actualState = await automation.GetInitialStateAsync();
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new SokobanState(8, 8, "#########ssssss##s#s$#s##s##.#s##@#.$ss##s##s#s##ssssss#########");
        int expectedHash = expectedState.GetStateHash();
        
        actualHash.Should().Be(expectedHash);
    }
    
    [Fact]
    public async Task ApplyMovesAsync()
    {
        const string url = "https://en.grandgames.net/sokoban/id195119";
        const string frameSelector = "#ggPuzzleFrame";
        const string canvasSelector = "#gamediv > canvas";
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        List<SokobanMove> moves = [
            new(SokobanMoveDirection.Up),
            new(SokobanMoveDirection.Up),
            new(SokobanMoveDirection.Up),
            new(SokobanMoveDirection.Right),
        ];
        
        IOptionsMonitor<SokobanAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<SokobanAutomationOptions>>();
        using var automation = new SokobanAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();
        await automation.ApplyMovesAsync(moves, cts.Token);

        string base64 = await Page.FrameLocator(frameSelector).Locator(canvasSelector).EvaluateAsync<string>("c => c.toDataURL()");
        byte[] bytes = Encoding.UTF8.GetBytes(base64);
        byte[] hashBytes = SHA256.HashData(bytes);
        string hash = Convert.ToHexString(hashBytes);
        string expectedHash = "E3D44721FC80D044C11C859E21E6A4BD17EE8191B48C92AC08EEF7E6342F5D18";
        
        hash.Should().Be(expectedHash);
    }
}
