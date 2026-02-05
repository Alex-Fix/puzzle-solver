using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PuzzleSolver.Core.BallSort.Solvers;

namespace PuzzleSolver.Core.BallSort;

internal static class DependencyInjection
{
    public static IServiceCollection AddBallSort(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<BallSortOptions>(configuration.GetSection(BallSortOptions.Name))
            .AddSingleton<IValidateOptions<BallSortOptions>, BallSortOptionsValidator>()
            .AddKeyedSingleton<IBallSortSolver, BeamSearchSolver>(BallSortAlgorithm.BeamSearch)
            .AddKeyedSingleton<IBallSortSolver, AStarSolver>(BallSortAlgorithm.AStar)
            .AddKeyedSingleton<IBallSortSolver, DfsSolver>(BallSortAlgorithm.Dfs)
            .AddKeyedSingleton<IBallSortSolver, BfsSolver>(BallSortAlgorithm.Bfs);
}
