using PuzzleSolver.Core.Interfaces;

namespace PuzzleSolver.Core.Nonogram.Solvers;

public interface INonogramSolver : ISolver<NonogramState, NonogramMove, NonogramOptions>;
