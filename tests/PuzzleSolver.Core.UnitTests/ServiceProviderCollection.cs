namespace PuzzleSolver.Core.UnitTests;

[CollectionDefinition(Name)]
public sealed class ServiceProviderCollection : IClassFixture<ServiceProviderFixture>
{
    public const string Name = nameof(ServiceProviderCollection);
}
