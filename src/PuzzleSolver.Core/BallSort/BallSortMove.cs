using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.BallSort;

public readonly record struct BallSortMove(int From, int To) : IMove;
