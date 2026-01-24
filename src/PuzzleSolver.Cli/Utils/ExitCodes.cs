namespace PuzzleSolver.Cli.Utils;

public static class ExitCodes
{
    public const int Unknown = -1;
    public const int Success = 0;
    public const int UrlMismatch = 10;
    public const int InitialStateNotFound = 20;
    public const int SolutionNotFound = 30;
    public const int Cancelled = 130;
}
