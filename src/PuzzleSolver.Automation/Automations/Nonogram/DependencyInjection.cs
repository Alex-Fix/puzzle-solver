using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PuzzleSolver.Automation.Automations.Nonogram;

internal static class DependencyInjection
{
    public static IServiceCollection AddNonogram(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<NonogramAutomationOptions>(configuration.GetSection(NonogramAutomationOptions.Name))
            .AddSingleton<IValidateOptions<NonogramAutomationOptions>, NonogramAutomationOptionsValidator>();
}
