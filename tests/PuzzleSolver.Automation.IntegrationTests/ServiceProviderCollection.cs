namespace PuzzleSolver.Automation.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ServiceProviderCollection : IClassFixture<ServiceProviderFixture>
{
    public const string Name = nameof(ServiceProviderCollection);
}
