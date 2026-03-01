using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Automation.AutomationFactories;
using PuzzleSolver.Automation.Automations.BallSort;
using PuzzleSolver.Automation.Automations.Sokoban;

namespace PuzzleSolver.Automation;

public static class DependencyInjection
{
    public static IServiceCollection AddAutomationServices(this IServiceCollection services,  IConfiguration configuration)
        => services
            .AddBallSort(configuration)
            .AddSokoban(configuration)
            .AddAutomationFactory(configuration);
}
