using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Core.BallSort;
using PuzzleSolver.Core.BallSort.Solvers;

namespace PuzzleSolver.Core.UnitTests.BallSort;

public class BallSortSolverTests
{
    [Theory]
    [InlineData(BallSortAlgorithm.BeamSearch)]
    public void Solve(BallSortAlgorithm algorithm)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { $"{nameof(BallSortOptions)}:{nameof(BallSortOptions.BeamWidth)}", "250" }
            })
            .Build();

        var solver = new ServiceCollection()
            .AddCoreServices(configuration)
            .BuildServiceProvider()
            .GetRequiredKeyedService<IBallSortSolver>(algorithm);
        
        var initialState = new BallSortState(21, 6, [[15,4],[15,9],[13,9,13,18],[10,8,7,8,6],[14,6,6,14,5],[5,17,17,5,14,4],[16,4,16,3,16,11],[6,7,9,6,7,17],[3,16,11,11,2,18],[18,1,1,0,1,10],[13,10,13,10,8,7],[13,15,15,13,10,15],[2,18,3,2,1,0],[4,3,11,3,16,2],[5,12,12,12,5,16],[11,2,11,18,2,1],[4,12,4,12,3,14],[9,10,9,9,8,15],[7,8,8,6,7,14],[18,0,0,0,1,0],[14,17,17,5,17,12]]);
        var moves = solver.Solve(initialState);

        int expectedCount = 407;
        
        moves.Should().HaveCount(expectedCount);
    }
}
