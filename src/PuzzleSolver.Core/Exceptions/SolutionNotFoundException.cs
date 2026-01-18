namespace PuzzleSolver.Core.Exceptions;

public sealed class SolutionNotFoundException : Exception
{
    public SolutionNotFoundException() : base("Solution not found.")
    {
    }
}
