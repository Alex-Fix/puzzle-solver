using FluentAssertions;
using PuzzleSolver.Core.Nonogram;
using PuzzleSolver.Core.Nonogram.Solvers;

namespace PuzzleSolver.Core.UnitTests.Nonogram;

public sealed class NonogramSolverTests : IClassFixture<ServiceProviderFixture>
{
    private readonly ServiceProviderFixture _serviceProviderFixture;

    public NonogramSolverTests(ServiceProviderFixture serviceProviderFixture)
        => _serviceProviderFixture = serviceProviderFixture;
    
    [Theory]
    [InlineData(NonogramAlgorithm.Smt)]
    public void Solve(NonogramAlgorithm algorithm)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        byte[][] rowClues =
        [
            [8],
            [1, 1],
            [5, 1],
            [2, 2, 1, 2],
            [4, 2, 2, 2],
            [5, 3, 2],
            [2, 4, 4],
            [3, 1],
            [6, 1, 1],
            [5, 1, 2, 2],
            [1, 2, 3, 3],
            [1, 3, 5],
            [4, 2, 2],
            [2, 3, 1],
            [5]
        ];

        byte[][] columnClues =
        [
            [5],
            [5, 1],
            [1, 2, 4, 1],
            [1, 3, 3, 2],
            [1, 4, 2, 3],
            [1, 4, 1, 3, 1],
            [1, 1, 2, 3, 2],
            [1, 4, 1, 1, 3],
            [1, 5, 1, 4],
            [1, 1, 2, 1, 1, 1],
            [1, 1, 1, 1, 3],
            [1, 2, 1, 3],
            [1, 1, 3],
            [3, 3],
            [2]
        ];
        
        INonogramSolver solver = _serviceProviderFixture.GetRequiredKeyedService<INonogramSolver>(algorithm);
        var state = new NonogramState(rowClues, columnClues);
        IEnumerable<NonogramMove> moves = solver.Solve(state, cts.Token);
        
        moves.Should().NotBeEmpty();
    }
}

