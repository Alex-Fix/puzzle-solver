namespace PuzzleSolver.Cli.FunctionalTests;

[CollectionDefinition(Name)]
public class CommandAppCollection : IClassFixture<CommandAppFixture>
{
    public const string Name = nameof(CommandAppCollection);
}
