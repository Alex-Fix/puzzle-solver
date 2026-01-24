using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Automation.AutomationFactories;

public sealed class AutomationFactoryOptions : IOptions
{
    public static string Name => nameof(AutomationFactoryOptions);

    public const bool DefaultHeadless = false;
    public bool Headless { get; set; } = DefaultHeadless;
}
