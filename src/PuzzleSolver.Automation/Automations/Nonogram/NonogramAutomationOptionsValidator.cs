using Microsoft.Extensions.Options;

namespace PuzzleSolver.Automation.Automations.Nonogram;

internal class NonogramAutomationOptionsValidator : IValidateOptions<NonogramAutomationOptions>
{
    public ValidateOptionsResult Validate(string? name, NonogramAutomationOptions options)
    {
        if (options.MoveDelayMs <= 0)
            return ValidateOptionsResult.Fail($"{nameof(options.MoveDelayMs)} must be greater than 0.");
        
        return ValidateOptionsResult.Success;
    }
}
