using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Nonogram;

public readonly record struct NonogramMove(int Row, int Column, bool Filled) : IMove;
