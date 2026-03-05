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
    [InlineData(NonogramAlgorithm.AStar)]
    public void Solve(NonogramAlgorithm algorithm)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        INonogramSolver solver = _serviceProviderFixture.GetRequiredKeyedService<INonogramSolver>(algorithm);
        
        int[][] columnClues =
        {
            new[] {6,20,6},              // 0
            new[] {5,5,2,3,4,6},         // 1
            new[] {3,4,8,5},             // 2
            new[] {2,5,3,3,4},           // 3
            new[] {1,5,1,4,2},           // 4
            new[] {4,3,6,1},             // 5
            new[] {2,1,4,4},             // 6
            new[] {2,6,5,4},             // 7
            new[] {1,6,6,11},            // 8
            new[] {1,5,5,3,3},           // 9
            new[] {1,4,2,2,2,2},         // 10
            new[] {1,2,4,4,6,1},         // 11
            new[] {2,1,1,2,1,3,5},       // 12
            new[] {2,2,3,3},             // 13
            new[] {3,4,1,2,1,2,1},       // 14
            new[] {2,5,1,4,1},           // 15
            new[] {2,6,1,4,2},           // 16
            new[] {1,5,3,2,2,3},         // 17
            new[] {1,5,1,1,3,7},         // 18
            new[] {2,3,1,4,6},           // 19
            new[] {2,1,3,4,1},           // 20
            new[] {3,1,2,4,2},           // 21
            new[] {2,1,2,4,4,7},         // 22
            new[] {2,2,3,2,2,9},         // 23
            new[] {1,2,4,1,8},           // 24
            new[] {1,5,4,2},             // 25
            new[] {1,5,6,5},             // 26
            new[] {1,4,6,8},             // 27
            new[] {2,4,5,11},            // 28
            new[] {2,2,1,2,10,1},        // 29
            new[] {3,1,2,4,2},           // 30
            new[] {1,3,2,2,5,3},         // 31
            new[] {2,6,11},              // 32
            new[] {3,5,2,6},             // 33
            new[] {6,9,4,6},             // 34
        };
        
        int[][] rowClues = new int[][]
        {
            new int[]{5,7,6,4},
            new int[]{4,3,3,3,2,3},
            new int[]{3,1,4,4,2,2},
            new int[]{2,1,3,8,2,2,1},
            new int[]{2,2,5,5,2,1},
            new int[]{1,1,6,7,2,1},
            new int[]{1,4,4,1},
            new int[]{2,3,5,1},
            new int[]{2,2,2,1},
            new int[]{2,1,2},
            new int[]{2,2},
            new int[]{3,2,2,2},
            new int[]{3,2,7,2,2,2},
            new int[]{2,5,6,9,2},
            new int[]{2,4,5,6,1},
            new int[]{2,4,3,4,3,5,1},
            new int[]{1,7,1,4,1,8,1},
            new int[]{2,4,5,1,5,5,1},
            new int[]{2,3,2,2,2,1},
            new int[]{1,1,1},
            new int[]{1,1,3},
            new int[]{2,1,1,2,2},
            new int[]{2,2,4,2},
            new int[]{2,3,2,3,1},
            new int[]{1,1,14,2,1},
            new int[]{1,2,2,3,5,3},
            new int[]{1,2,2,5,1,3},
            new int[]{3,6,2,3},
            new int[]{3,2,3,2,3},
            new int[]{4,2,2,3},
            new int[]{4,2,7},
            new int[]{1,4,1,1,1,6,1},
            new int[]{1,2,1,1,1,2,6},
            new int[]{2,1,1,2,3,6,1},
            new int[]{2,5,1,2,3,4,2,1},
            new int[]{3,4,2,2,3,4,2},
            new int[]{4,4,2,2,3,3,3},
            new int[]{4,4,2,3,7,4},
            new int[]{5,3,2,2,7,5},
            new int[]{6,6,2,5,6}
        };
        
        var initialState = new NonogramState(40, 35, columnClues, rowClues);
        IEnumerable<NonogramMove> moves = solver.Solve(initialState, cts.Token);

        moves.Should().NotBeEmpty();
    }
}

