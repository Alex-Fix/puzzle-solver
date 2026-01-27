using FluentAssertions;
using PuzzleSolver.Automation.Automations.BallSort;
using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Automation.Interfaces;

namespace PuzzleSolver.Automation.IntegrationTests.AutomationFactories;

[Collection(ServiceProviderCollection.Name)]
public sealed class PlaywrightAutomationFactoryTests
{
    private readonly ServiceProviderFixture _serviceProviderFixture;

    public PlaywrightAutomationFactoryTests(ServiceProviderFixture serviceProviderFixture)
        => _serviceProviderFixture = serviceProviderFixture;

    [Fact]
    public async Task InstallAsync()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        IAutomationFactory factory = _serviceProviderFixture.GetRequiredService<IAutomationFactory>();
        
        await factory
            .Invoking(f => f.InstallAsync(cts.Token))
            .Should()
            .NotThrowAsync<DriverInstallationException>();
    }
    
    [Fact]
    public async Task CreateAsync_WhenBallSortAutomation()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        IAutomationFactory factory = _serviceProviderFixture.GetRequiredService<IAutomationFactory>();
        await factory.InstallAsync(cts.Token);
        using BallSortAutomation automation = await factory.CreateAsync<BallSortAutomation>(cts.Token);
        
        automation.Should().NotBeNull();
        automation.Should().BeOfType<BallSortAutomation>();
    }
}
