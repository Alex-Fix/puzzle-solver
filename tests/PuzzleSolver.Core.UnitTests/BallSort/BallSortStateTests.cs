using FluentAssertions;
using PuzzleSolver.Core.BallSort;

namespace PuzzleSolver.Core.UnitTests.BallSort;

public sealed class BallSortStateTests
{
    [Fact]
    public void GetValidMoves_WhenNotFull()
    {
        var state = new BallSortState(4, 3, [[], [1,2], [1,2], [1,2]]);
        IEnumerable<BallSortMove> moves = state.GetValidMoves();

        List<BallSortMove> expectedMoves = [
            new(1, 0),
            new(1, 2), 
            new(1, 3),
            new(2, 0),
            new(2, 1),
            new(2, 3),
            new(3, 0), 
            new(3, 1),
            new(3, 2)
        ];
        
        moves.Should().BeEquivalentTo(expectedMoves);
    }
    
    [Fact]
    public void GetValidMoves_WhenFull()
    {
        var state = new BallSortState(4, 3, [[], [1,1,2], [1,2,2], [3,3,3]]);
        IEnumerable<BallSortMove> moves = state.GetValidMoves();

        List<BallSortMove> expectedMoves = [
            new(1, 0),
            new(2, 0),
            new(3, 0)
        ];
        
        moves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void Apply()
    {
        var actualState = new BallSortState(4, 3, [[], [0, 0, 0], [1, 2, 2], [1, 1, 2]]);
        var move = new BallSortMove(1, 0);
        actualState = actualState.Apply(move);
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new BallSortState(4, 3, [[0], [0, 0], [1, 2, 2], [1, 1, 2]]);
        int expectedHash = expectedState.GetStateHash();
        
        actualHash.Should().Be(expectedHash);
    }

    [Fact]
    public void IsSolved_Solved()
    {
        var state = new BallSortState(4, 3, [[1,1,1], [0,0,0], [2,2,2], []]);
        bool isSolved = state.IsSolved();
        
        isSolved.Should().BeTrue();
    }

    [Fact]
    public void IsSolved_NotSolved()
    {
        var state = new BallSortState(4, 3, [[1], [1, 1], [0, 0, 0], [2, 2, 2]]);
        bool isSolved = state.IsSolved();
        
        isSolved.Should().BeFalse();
    }

    [Fact]
    public void GetHeuristic_WhenFirstLess()
    {
        var options = new BallSortOptions();
        
        var actualState = new BallSortState(3, 3, [[0, 0, 0], [1, 1, 2], [2, 2, 1]]);
        double actualHeuristic = actualState.GetHeuristic(options);
        
        var expectedState = new BallSortState(3, 3, [[0, 0, 1], [0, 1, 2], [2, 2, 1]]);
        double expectedHeuristic = expectedState.GetHeuristic(options);

        actualHeuristic.Should().BeLessThan(expectedHeuristic);
    }

    [Fact]
    public void GetHeuristic_WhenExactValue()
    {
        var options = new BallSortOptions();
        var state =  new BallSortState(4, 3, [[0, 0, 0], [1, 2, 1], [2, 3, 3], [1, 2, 3]]);
        double heuristic = state.GetHeuristic(options);
        
        double expectedHeuristic = 5;
        
        heuristic.Should().Be(expectedHeuristic);
    }

    [Fact]
    public void GetStateHash_WhenEqual()
    {
        var actualState = new BallSortState(3, 3, [[0, 0, 0], [1, 2, 1], [2, 3, 3]]);
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new BallSortState(3, 3, [[0, 0, 0], [1, 2, 1], [2, 3, 3]]);
        int expectedHash = expectedState.GetStateHash();

        actualHash.Should().Be(expectedHash);
    }

    [Fact]
    public void GetStateHash_WhenNotEqual()
    {
        var actualState = new BallSortState(3, 3, [[1, 0, 0], [1, 2, 1], [2, 3, 3]]);
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new BallSortState(3, 3, [[0, 0, 0], [1, 2, 1], [2, 3, 3]]);
        int expectedHash = expectedState.GetStateHash();

        actualHash.Should().NotBe(expectedHash);
    }
}
