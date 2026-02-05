using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PuzzleSolver.Core.Sokoban.Solvers;

namespace PuzzleSolver.Core.Sokoban;

internal static class DependencyInjection
{
    public static IServiceCollection AddSokoban(this IServiceCollection serviceCollection, IConfiguration configuration)
        => serviceCollection
            .Configure<SokobanOptions>(configuration.GetSection(SokobanOptions.Name))
            .AddSingleton<IValidateOptions<SokobanOptions>, SokobanOptionsValidator>()
            .AddKeyedSingleton<ISokobanSolver, SokobanBeamSearchSolver>(SokobanAlgorithm.BeamSearch);
}
