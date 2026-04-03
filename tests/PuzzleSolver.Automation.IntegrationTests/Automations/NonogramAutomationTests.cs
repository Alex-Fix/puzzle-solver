using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.Playwright.Xunit;
using PuzzleSolver.Automation.Automations.Nonogram;
using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Core.Nonogram;

namespace PuzzleSolver.Automation.IntegrationTests.Automations;

public class NonogramAutomationTests : PageTest, IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture _serviceProviderFixture;
    
    public NonogramAutomationTests(ServiceProviderFixture serviceProviderFixture)
        => _serviceProviderFixture = serviceProviderFixture;
    
    [Fact]
    public async Task NavigateAsync_WhenWrongUrl()
    {
        const string url = "https://google.com";
        
        IOptionsMonitor<NonogramAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<NonogramAutomationOptions>>();
        using var automation = new NonogramAutomation(Page, optionsMonitor);
        
        await automation
            .Invoking(a => a.NavigateAsync(url))
            .Should()
            .ThrowAsync<UrlMismatchException>();
    }
    
    [Fact]
    public async Task NavigateAsync_WhenValidUrl()
    {
        const string url = "https://www.nonograms.ru/nonograms/i/80309";
        
        IOptionsMonitor<NonogramAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<NonogramAutomationOptions>>();
        using var automation = new NonogramAutomation(Page, optionsMonitor);
        
        await automation
            .Invoking(a => a.NavigateAsync(url))
            .Should()
            .NotThrowAsync();
    }
    
    [Fact]
    public async Task ConfigureAsync()
    {
        const string url = "https://www.nonograms.ru/nonograms/i/80309";
        const string gameContainerSelector = ".nonogram_table";
        
        IOptionsMonitor<NonogramAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<NonogramAutomationOptions>>();
        using var automation = new NonogramAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();
    
        Page.Url.Should().Be(url);
        
        bool isVisible = await Page.IsVisibleAsync(gameContainerSelector);
        isVisible.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetInitialStateAsync()
    {
        const string url = "https://www.nonograms.ru/nonograms/i/80191";
        
        IOptionsMonitor<NonogramAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<NonogramAutomationOptions>>();
        using var automation = new NonogramAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();
        
        NonogramState actualState = await automation.GetInitialStateAsync();
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new NonogramState([[8],[1,1],[5,1],[2,2,1,2],[4,2,2,2],[5,3,2],[2,4,4],[3,1],[6,1,1],[5,1,2,2],[1,2,3,3],[1,3,5],[4,2,2],[2,3,1],[5]], [[5],[5,1],[1,2,4,1],[1,3,3,2],[1,4,2,3],[1,4,1,3,1],[1,1,2,3,2],[1,4,1,1,3],[1,5,1,4],[1,1,2,1,1,1],[1,1,1,1,3],[1,2,1,3],[1,1,3],[3,3],[2]]);
        int expectedHash = expectedState.GetStateHash();
        
        actualHash.Should().Be(expectedHash);
    }
    
    [Fact]
    public async Task ApplyMovesAsync()
    {
        const string url = "https://www.nonograms.ru/nonograms/i/80191";
        const string firstCellSelector = "#nmf0_0";
        const string secondCellSelector = "#nmf1_1";
        const string styleAttribute = "style";
        const string filledCellStyle = "background-color: rgb(0, 0, 0);";
        
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        List<NonogramMove> moves = [
            new(0, 0, true),
            new(1, 1, true),
        ];
        
        IOptionsMonitor<NonogramAutomationOptions> optionsMonitor = _serviceProviderFixture.GetRequiredService<IOptionsMonitor<NonogramAutomationOptions>>();
        using var automation = new NonogramAutomation(Page, optionsMonitor);
        
        await automation.NavigateAsync(url);
        await automation.ConfigureAsync();
        await automation.ApplyMovesAsync(moves, cts.Token);

        string? firstCellStyle = await Page.Locator(firstCellSelector).First.GetAttributeAsync(styleAttribute);
        string? secondCellStyle = await Page.Locator(secondCellSelector).First.GetAttributeAsync(styleAttribute);

        firstCellStyle.Should().Contain(filledCellStyle);
        secondCellStyle.Should().Contain(filledCellStyle);
    }
}
