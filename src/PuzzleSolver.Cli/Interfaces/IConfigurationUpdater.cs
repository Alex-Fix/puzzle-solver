using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Cli.Interfaces;

public interface IConfigurationUpdater
{
    void Update(params IOptions[] options);
}
