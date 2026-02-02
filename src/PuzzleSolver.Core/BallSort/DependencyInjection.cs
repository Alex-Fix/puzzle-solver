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
            .AddKeyedSingleton<IBallSortSolver, BeamSearchBallSortSolver>(BallSortAlgorithm.BeamSearch)
            .AddKeyedSingleton<IBallSortSolver, AStarBallSortSolver>(BallSortAlgorithm.AStar)
            .AddKeyedSingleton<IBallSortSolver, DfsBallSortSolver>(BallSortAlgorithm.Dfs)
            .AddKeyedSingleton<IBallSortSolver, BfsBallSortSolver>(BallSortAlgorithm.Bfs);
}
