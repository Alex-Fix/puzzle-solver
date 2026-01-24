using Microsoft.Extensions.Options;

namespace PuzzleSolver.Automation.AutomationFactories;

internal sealed class AutomationFactoryOptionsValidator : IValidateOptions<AutomationFactoryOptions>
{
    public ValidateOptionsResult Validate(string? name, AutomationFactoryOptions options)
        => ValidateOptionsResult.Success;
}
