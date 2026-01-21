using FluentAssertions;
using PuzzleSolver.Automation.Automations.BallSort;
using PuzzleSolver.Automation.Interfaces;

namespace PuzzleSolver.Automation.IntegrationTests.AutomationFactories;

public sealed class PlaywrightAutomationFactoryTests : IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture _serviceProviderFixture;

    public PlaywrightAutomationFactoryTests(ServiceProviderFixture serviceProviderFixture)
    {
        _serviceProviderFixture = serviceProviderFixture;
    }

    [Fact]
    public async Task CreateAsync_WhenBallSortAutomation()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        
        IAutomationFactory factory = _serviceProviderFixture.GetRequiredService<IAutomationFactory>();
        using BallSortAutomation automation = await factory.CreateAsync<BallSortAutomation>(cts.Token);

        automation.Should().NotBeNull();
        automation.Should().BeOfType<BallSortAutomation>();
    }
}
