using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PuzzleSolver.Automation.Automations.BallSort;

internal static class DependencyInjection
{
    public static IServiceCollection AddBallSort(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<BallSortAutomationOptions>(configuration.GetSection(BallSortAutomationOptions.Name))
            .AddSingleton<IValidateOptions<BallSortAutomationOptions>, BallSortAutomationOptionsValidator>();
}
