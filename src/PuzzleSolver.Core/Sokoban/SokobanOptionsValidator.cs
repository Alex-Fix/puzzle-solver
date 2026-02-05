using Microsoft.Extensions.Options;

namespace PuzzleSolver.Core.Sokoban;

internal sealed class SokobanOptionsValidator : IValidateOptions<SokobanOptions>
{
    public ValidateOptionsResult Validate(string? name, SokobanOptions options)
        => ValidateOptionsResult.Success;
}
