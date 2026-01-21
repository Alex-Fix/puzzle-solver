using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Automation.Automations.BallSort;

namespace PuzzleSolver.Automation.IntegrationTests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ServiceProviderFixture : IAsyncLifetime
{
    private ServiceProvider? _serviceProvider;

    public T GetRequiredService<T>() where T : notnull
    {
        if(_serviceProvider is null)
            throw new NullReferenceException(nameof(_serviceProvider));
        
        return _serviceProvider.GetRequiredService<T>();
    }

    public Task InitializeAsync()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { $"{nameof(BallSortAutomationOptions)}:{nameof(BallSortAutomationOptions.MoveDelayMs)}", 600.ToString() } 
            })
            .Build();
        
        _serviceProvider = new ServiceCollection()
            .AddAutomationServices(configuration)
            .BuildServiceProvider();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is null)
            return;

        await _serviceProvider.DisposeAsync();
    }
}
