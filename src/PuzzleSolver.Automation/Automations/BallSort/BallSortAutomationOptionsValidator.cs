using Microsoft.Extensions.Options;

namespace PuzzleSolver.Automation.Automations.BallSort;

internal class BallSortAutomationOptionsValidator : IValidateOptions<BallSortAutomationOptions>
{
    public ValidateOptionsResult Validate(string? name, BallSortAutomationOptions options)
    {
        if (options.MoveDelayMs <= 0)
            return ValidateOptionsResult.Fail($"{nameof(options.MoveDelayMs)} must be greater than 0.");

        return ValidateOptionsResult.Success;
    }
}
