using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Core.BallSort;

namespace PuzzleSolver.Core.UnitTests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ServiceProviderFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public ServiceProviderFixture()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { $"{nameof(BallSortOptions)}:{nameof(BallSortOptions.BeamWidth)}", 250.ToString() }
            })
            .Build();
        
        _serviceProvider = new ServiceCollection()
            .AddCoreServices(configuration)
            .BuildServiceProvider();
    }

    public T GetRequiredKeyedService<T>(object? key) where T : notnull
        => _serviceProvider.GetRequiredKeyedService<T>(key);
    
    public void Dispose()
        => _serviceProvider.Dispose();
}
