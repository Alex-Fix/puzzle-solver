namespace PuzzleSolver.Automation.Exceptions;

public sealed class DriverInstallationException : Exception
{
    public DriverInstallationException(string driverName) : base($"Unable to install {driverName}.")
    {
        DriverName = driverName;
    }
    
    public string DriverName { get; }
}
