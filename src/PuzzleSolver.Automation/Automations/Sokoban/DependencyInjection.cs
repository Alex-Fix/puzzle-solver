using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PuzzleSolver.Automation.Automations.Sokoban;

internal static class DependencyInjection
{
    public static IServiceCollection AddSokoban(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<SokobanAutomationOptions>(configuration.GetSection(SokobanAutomationOptions.Name))
            .AddSingleton<IValidateOptions<SokobanAutomationOptions>, SokobanAutomationOptionsValidator>();
}
