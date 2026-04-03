using Microsoft.Extensions.Options;

namespace PuzzleSolver.Core.Nonogram;

internal sealed class NonogramOptionsValidator : IValidateOptions<NonogramOptions>
{
    public ValidateOptionsResult Validate(string? name, NonogramOptions options)
        => ValidateOptionsResult.Success;
}
