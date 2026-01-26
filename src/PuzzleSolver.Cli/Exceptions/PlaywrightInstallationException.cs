namespace PuzzleSolver.Cli.Exceptions;

public sealed class PlaywrightInstallationException : Exception
{
    public PlaywrightInstallationException() : base("Unable to install Playwright and browser dependencies.")
    {
    }
}
