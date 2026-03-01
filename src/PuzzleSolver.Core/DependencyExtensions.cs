using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Core.BallSort;
using PuzzleSolver.Core.Sokoban;

namespace PuzzleSolver.Core;

public static class DependencyExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddBallSort(configuration)
            .AddSokoban(configuration);
}
