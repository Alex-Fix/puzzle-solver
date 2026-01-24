using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PuzzleSolver.Automation.Interfaces;

namespace PuzzleSolver.Automation.AutomationFactories;

internal static class DependencyInjection
{
    public static IServiceCollection AddAutomationFactory(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<AutomationFactoryOptions>(configuration.GetSection(AutomationFactoryOptions.Name))
            .AddSingleton<IValidateOptions<AutomationFactoryOptions>, AutomationFactoryOptionsValidator>()
            .AddSingleton<IAutomationFactory, PlaywrightAutomationFactory>();
}
