using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Sokoban.Solvers;

public interface ISokobanSolver : ISolver<SokobanState, SokobanMove, SokobanOptions>;
