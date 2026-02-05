using FluentAssertions;
using PuzzleSolver.Core.Sokoban;
using PuzzleSolver.Core.Sokoban.Solvers;

namespace PuzzleSolver.Core.UnitTests.Sokoban;

public sealed class SokobanSolverTests : IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture _serviceProviderFixture;

    public SokobanSolverTests(ServiceProviderFixture serviceProviderFixture)
        => _serviceProviderFixture = serviceProviderFixture;
    
    [Theory]
    [InlineData(SokobanAlgorithm.BeamSearch)]
    public void Solve(SokobanAlgorithm algorithm)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        ISokobanSolver solver = _serviceProviderFixture.GetRequiredKeyedService<ISokobanSolver>(algorithm);
        
        var initialState = new SokobanState(9, 10, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########");
        IEnumerable<SokobanMove> moves = solver.Solve(initialState, cts.Token);

        moves.Should().NotBeEmpty();
    }
}
