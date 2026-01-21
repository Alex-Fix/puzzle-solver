using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PuzzleSolver.Core.BallSort.Solvers;

namespace PuzzleSolver.Core.BallSort;

internal static class DependencyInjection
{
    public static IServiceCollection AddBallSort(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<BallSortOptions>(configuration.GetRequiredSection(nameof(BallSortOptions)))
            .AddSingleton<IValidateOptions<BallSortOptions>, BallSortOptionsValidator>()
            .AddKeyedSingleton<IBallSortSolver, BeamSearchBallSortSolver>(BallSortAlgorithm.BeamSearch);
}
