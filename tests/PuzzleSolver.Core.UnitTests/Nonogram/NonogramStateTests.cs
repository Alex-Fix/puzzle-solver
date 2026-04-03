using FluentAssertions;
using PuzzleSolver.Core.Nonogram;

namespace PuzzleSolver.Core.UnitTests.Nonogram;

public sealed class NonogramStateTests
{
    private readonly byte[][] _simpleRows = [[1], [1]];
    private readonly byte[][] _simpleCols = [[1], [1]];

    [Fact]
    public void GetValidMoves_ReturnsBothOptions_WhenCellIsUnknown()
    {
        var state = new NonogramState(_simpleRows, _simpleCols);

        var moves = state.GetValidMoves().ToList();

        moves.Should().HaveCount(2);
        moves.Should().Contain(m => m.Row == 0 && m.Column == 0 && m.Filled);
        moves.Should().Contain(m => m.Row == 0 && m.Column == 0 && !m.Filled);
    }

    [Fact]
    public void GetValidMoves_PrunesMoves_WhenClueIsExceeded()
    {
        var state = new NonogramState([[1]], [[1], [0]]);
        var firstMove = new NonogramMove(0, 0, true);
        state = state.Apply(firstMove);

        var moves = state.GetValidMoves().ToList();

        moves.Should().HaveCount(1);
        moves.Should().Contain(m => m.Row == 0 && m.Column == 1 && !m.Filled);
    }

    [Fact]
    public void Apply_ReturnsNewState_WithUpdatedHash()
    {
        var state = new NonogramState(_simpleRows, _simpleCols);
        int initialHash = state.GetStateHash();
        var move = new NonogramMove(0, 0, true);

        var nextState = state.Apply(move);

        nextState.GetStateHash().Should().NotBe(initialHash);
    }

    [Fact]
    public void IsSolved_ReturnsTrue_WhenGridMatchesClues()
    {
        var state = new NonogramState(_simpleRows, _simpleCols);

        state = state.Apply(new NonogramMove(0, 0, true));
        state = state.Apply(new NonogramMove(0, 1, false));
        state = state.Apply(new NonogramMove(1, 0, false));
        state = state.Apply(new NonogramMove(1, 1, true));

        state.IsSolved().Should().BeTrue();
    }

    [Fact]
    public void IsSolved_ReturnsFalse_WhenUnknownCellsExist()
    {
        var state = new NonogramState(_simpleRows, _simpleCols);

        state.IsSolved().Should().BeFalse();
    }

    [Fact]
    public void IsSolved_ReturnsFalse_WhenCluesAreNotMet()
    {
        var state = new NonogramState(_simpleRows, _simpleCols);
        
        state = state.Apply(new NonogramMove(0, 0, true));
        state = state.Apply(new NonogramMove(0, 1, true));
        state = state.Apply(new NonogramMove(1, 0, true));
        state = state.Apply(new NonogramMove(1, 1, true));

        state.IsSolved().Should().BeFalse();
    }

    [Fact]
    public void GetStateHash_IsConsistentForSameState()
    {
        var state1 = new NonogramState(_simpleRows, _simpleCols);
        state1 = state1.Apply(new NonogramMove(0, 0, true));

        var state2 = new NonogramState(_simpleRows, _simpleCols);
        state2 = state2.Apply(new NonogramMove(0, 0, true));

        state1.GetStateHash().Should().Be(state2.GetStateHash());
    }

    [Fact]
    public void GetHeuristic_ReturnsRemainingUnknownCount()
    {
        var state = new NonogramState(_simpleRows, _simpleCols);
        
        double initialHeuristic = state.GetHeuristic(default!);
        state = state.Apply(new NonogramMove(0, 0, true));
        double nextHeuristic = state.GetHeuristic(default!);

        initialHeuristic.Should().Be(4);
        nextHeuristic.Should().Be(3);
    }
}
