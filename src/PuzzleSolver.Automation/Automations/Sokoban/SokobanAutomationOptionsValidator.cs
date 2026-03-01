using Microsoft.Extensions.Options;

namespace PuzzleSolver.Automation.Automations.Sokoban;

internal class SokobanAutomationOptionsValidator : IValidateOptions<SokobanAutomationOptions>
{
    public ValidateOptionsResult Validate(string? name, SokobanAutomationOptions options)
    {
        if(options.MoveDelayMs <= 0)
            return ValidateOptionsResult.Fail($"{nameof(options.MoveDelayMs)} must be greater than 0.");

        return ValidateOptionsResult.Success;
    }
}
