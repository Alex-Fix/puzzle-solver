namespace PuzzleSolver.Automation.Exceptions;

public sealed class InitialStateNotFoundException : Exception
{
    public InitialStateNotFoundException(string message) : base(message)
    {
    }
}
