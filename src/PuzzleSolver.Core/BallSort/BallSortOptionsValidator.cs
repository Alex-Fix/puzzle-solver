using Microsoft.Extensions.Options;

namespace PuzzleSolver.Core.BallSort;

internal sealed class BallSortOptionsValidator : IValidateOptions<BallSortOptions>
{
    public ValidateOptionsResult Validate(string? name, BallSortOptions options)
    {
        if (options.BeamWidth <= 0)
            return ValidateOptionsResult.Fail($"{nameof(options.BeamWidth)} must be greater than 0.");

        return ValidateOptionsResult.Success;
    }
}
