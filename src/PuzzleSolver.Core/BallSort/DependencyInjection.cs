using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PuzzleSolver.Core.BallSort.Solvers;

namespace PuzzleSolver.Core.BallSort;

public static class DependencyInjection
{
    public static IServiceCollection AddBallSort(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<BallSortOptions>(configuration.GetSection(nameof(BallSortOptions)))
            .AddSingleton<IValidateOptions<BallSortOptions>,  BallSortOptionsValidator>()
            .AddKeyedSingleton<IBallSortSolver, BeamSearchBallSortSolver>(BallSortAlgorithm.BeamSearch)
            .AddSingleton<IBallSortSolver>(sp => sp.GetRequiredKeyedService<IBallSortSolver>(BallSortAlgorithm.BeamSearch));
    }
}
