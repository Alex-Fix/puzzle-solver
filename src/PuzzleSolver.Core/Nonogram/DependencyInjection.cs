using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Nonogram.Solvers;

namespace PuzzleSolver.Core.Nonogram;

internal static class DependencyInjection
{
    public static IServiceCollection AddNonogram(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<NonogramOptions>(configuration.GetSection(NonogramOptions.Name))
            .AddSingleton<IValidateOptions<NonogramOptions>, NonogramOptionsValidator>()
            .AddKeyedSingleton<INonogramSolver, SmtSolver>(NonogramAlgorithm.Smt)
            .AddKeyedSingleton<INonogramSolver, AStarSolver>(NonogramAlgorithm.AStar);
}
