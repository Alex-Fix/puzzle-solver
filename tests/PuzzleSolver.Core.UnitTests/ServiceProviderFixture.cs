using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PuzzleSolver.Core.UnitTests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ServiceProviderFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public ServiceProviderFixture()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder().Build();
        
        _serviceProvider = new ServiceCollection()
            .AddCoreServices(configuration)
            .BuildServiceProvider();
    }

    public T GetRequiredKeyedService<T>(object? key) where T : notnull
        => _serviceProvider.GetRequiredKeyedService<T>(key);
    
    public void Dispose()
        => _serviceProvider.Dispose();
}
