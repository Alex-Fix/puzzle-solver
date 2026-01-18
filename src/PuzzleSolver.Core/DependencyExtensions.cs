using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Core.BallSort;

namespace PuzzleSolver.Core;

public static class DependencyExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddBallSort(configuration);
    }
}
