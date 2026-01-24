namespace PuzzleSolver.Automation.Exceptions;

public sealed class UrlMismatchException : Exception
{
    public  UrlMismatchException(string urlStart) : base($"Url must start with {urlStart}.")
        => UrlStart = urlStart;
    
    public string UrlStart { get; }
}
