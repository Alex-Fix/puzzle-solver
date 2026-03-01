using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Sokoban;

public readonly record struct SokobanMove(SokobanMoveDirection Direction) : IMove;
