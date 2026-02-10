using FluentAssertions;
using PuzzleSolver.Core.Sokoban;

namespace PuzzleSolver.Core.UnitTests.Sokoban;

public sealed class SokobanStateTests
{
    [Fact]
    public void GetValidMoves_WhenUpRightDown()
    {
        var actualState = new SokobanState(9, 10, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########");
        IEnumerable<SokobanMove> actualMoves = actualState.GetValidMoves();

        IEnumerable<SokobanMove> expectedMoves =
        [
            new(SokobanMoveDirection.Up),
            new(SokobanMoveDirection.Right),
            new(SokobanMoveDirection.Down)
        ];
        
        actualMoves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void GetValidMoves_WhenUpRightDownLeft()
    {
        var actualState = new SokobanState(9, 10, "!!########!!#.s##ss#!!#...$@s#!##s.##ss#!#s$.$ss####s##$#ss##ssss$s$s##sssss#ss###########");
        IEnumerable<SokobanMove> actualMoves = actualState.GetValidMoves();

        IEnumerable<SokobanMove> expectedMoves =
        [
            new(SokobanMoveDirection.Up),
            new(SokobanMoveDirection.Right),
            new(SokobanMoveDirection.Down),
            new(SokobanMoveDirection.Left)
        ];
        
        actualMoves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void GetValidMoves_WhenRightLeft()
    {
        var actualState = new SokobanState(9, 10, "!!########!!#.s##ss#!!#..*@ss#!##s.##ss#!#s$.$ss####s##$#ss##ssss$s$s##sssss#ss###########");
        IEnumerable<SokobanMove> actualMoves = actualState.GetValidMoves();

        IEnumerable<SokobanMove> expectedMoves =
        [
            new(SokobanMoveDirection.Right),
            new(SokobanMoveDirection.Left)
        ];
        
        actualMoves.Should().BeEquivalentTo(expectedMoves);
    }
    
    [Theory]
    [InlineData(9, 10, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########", SokobanMoveDirection.Up, "!!########!!#.s##ss#!!#...sss#!##s.##$s#!#s$.$s@####s##$#ss##ssss$s$s##sssss#ss###########")]
    [InlineData(9, 10, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########", SokobanMoveDirection.Right, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#s@##ssss$s$s##sssss#ss###########")]
    [InlineData(9, 10, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########", SokobanMoveDirection.Down, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#ss##ssss$s@s##sssss#$s###########")]
    [InlineData(9, 10, "!!########!!#.s##ss#!!#...$@s#!##s.##ss#!#s$.$ss####s##$#ss##ssss$s$s##sssss#ss###########", SokobanMoveDirection.Left, "!!########!!#.s##ss#!!#..*@ss#!##s.##ss#!#s$.$ss####s##$#ss##ssss$s$s##sssss#ss###########")]
    [InlineData(9, 10, "!!########!!#.s##ss#!!#..*@ss#!##s.##ss#!#s$.$ss####s##$#ss##ssss$s$s##sssss#ss###########", SokobanMoveDirection.Left, "!!########!!#.s##ss#!!#.*+sss#!##s.##ss#!#s$.$ss####s##$#ss##ssss$s$s##sssss#ss###########")]
    public void Apply(int height, int width, string actualLayout, SokobanMoveDirection direction, string expectedLayout)
    {
        var actualState = new SokobanState(height, width, actualLayout);
        var move = new SokobanMove(direction);
        actualState = actualState.Apply(move);
        int actualHash =  actualState.GetStateHash();

        var expectedState = new SokobanState(height, width, expectedLayout);
        int expectedHash = expectedState.GetStateHash();
        
        actualHash.Should().Be(expectedHash);
    }
    
    [Theory]
    [InlineData(1, 7, "#.s@*+!", true)]
    [InlineData(1, 8, "#.s@$*+!", false)]
    public void IsSolved(int height, int width, string layout, bool expected)
    {
        var actualState = new SokobanState(height, width, layout);
        bool actual = actualState.IsSolved();
        
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void GetStateHash_WhenEqual()
    {
        var actualState = new SokobanState(9, 10, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########");
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new SokobanState(9, 10, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########");
        int expectedHash = expectedState.GetStateHash();
        
        actualHash.Should().Be(expectedHash);
    }

    [Fact]
    public void GetStateHash_WhenNotEqual()
    {
        var actualState = new SokobanState(9, 10, "!!########!!#.s##ss#!!#...sss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########");
        int actualHash = actualState.GetStateHash();
        
        var expectedState = new SokobanState(9, 10, "!!########!!#.s##ss#!!#...$ss#!##s.##ss#!#s$.$s$####s##$#@s##ssss$s$s##sssss#ss###########");
        int expectedHash = expectedState.GetStateHash();
        
        actualHash.Should().NotBe(expectedHash);
    }
}
