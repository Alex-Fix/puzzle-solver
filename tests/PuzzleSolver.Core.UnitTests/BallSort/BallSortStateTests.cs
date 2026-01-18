using FluentAssertions;
using PuzzleSolver.Core.BallSort;

namespace PuzzleSolver.Core.UnitTests.BallSort;

public class BallSortStateTests
{
    [Fact]
    public void GetValidMoves_WhenNotFull()
    {
        var state = new BallSortState(4, 3, [[], [1,2], [1,2], [1,2]]);
        var moves = state.GetValidMoves();

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
        var moves = state.GetValidMoves();

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
        var state1 = new BallSortState(4, 3, [[], [0, 0, 0], [1, 2, 2], [1, 1, 2]]);
        var state2 = new BallSortState(4, 3, [[0], [0, 0], [1, 2, 2], [1, 1, 2]]);
        var hash2 = state2.GetStateHash();
        
        var move = new BallSortMove(1, 0);
        state1 =  state1.Apply(move);
        var hash1 = state1.GetStateHash();
        
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void IsSolved_Solved()
    {
        var state = new BallSortState(4, 3, [[1,1,1], [0,0,0], [2,2,2], []]);
        var isSolved = state.IsSolved();
        
        isSolved.Should().BeTrue();
    }

    [Fact]
    public void IsSolved_NotSolved()
    {
        var state = new BallSortState(4, 3, [[1], [1, 1], [0, 0, 0], [2, 2, 2]]);
        var isSolved = state.IsSolved();
        
        isSolved.Should().BeFalse();
    }

    [Fact]
    public void GetHeuristic_WhenFirstLess()
    {
        var options = new BallSortOptions();
        
        var state1 = new BallSortState(3, 3, [[0, 0, 0], [1, 1, 2], [2, 2, 1]]);
        var heuristic1 = state1.GetHeuristic(options);
        
        var state2 = new BallSortState(3, 3, [[0, 0, 1], [0, 1, 2], [2, 2, 1]]);
        var heuristic2 = state2.GetHeuristic(options);

        heuristic1.Should().BeLessThan(heuristic2);
    }

    [Fact]
    public void GetHeuristic_WhenExactValue()
    {
        var options = new BallSortOptions();
        var state =  new BallSortState(4, 3, [[0, 0, 0], [1, 2, 1], [2, 3, 3], [1, 2, 3]]);
        var heuristic = state.GetHeuristic(options);
        
        var expectedHeuristic = 5;
        heuristic.Should().Be(expectedHeuristic);
    }

    [Fact]
    public void GetStateHash_WhenEqual()
    {
        var state1 = new BallSortState(3, 3, [[0, 0, 0], [1, 2, 1], [2, 3, 3]]);
        var hash1 = state1.GetStateHash();
        
        var state2 = new BallSortState(3, 3, [[0, 0, 0], [1, 2, 1], [2, 3, 3]]);
        var hash2 = state2.GetStateHash();

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetStateHash_WhenNotEqual()
    {
        var state1 = new BallSortState(3, 3, [[1, 0, 0], [1, 2, 1], [2, 3, 3]]);
        var hash1 = state1.GetStateHash();
        
        var state2 = new BallSortState(3, 3, [[0, 0, 0], [1, 2, 1], [2, 3, 3]]);
        var hash2 = state2.GetStateHash();

        hash1.Should().NotBe(hash2);
    }
}
