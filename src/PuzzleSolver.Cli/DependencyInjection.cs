using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Cli.Infrastructure;
using PuzzleSolver.Cli.Interfaces;

namespace PuzzleSolver.Cli;

public static class DependencyInjection
{
    public static IServiceCollection AddCliServices(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddSingleton(configuration)
            .AddSingleton<IExceptionHandler, ConsoleExceptionHandler>()
            .AddSingleton<IConfigurationUpdater, ReflectionConfigurationUpdater>();
}
