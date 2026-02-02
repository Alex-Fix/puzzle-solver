using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuzzleSolver.Core;
using PuzzleSolver.Core.BallSort;
using PuzzleSolver.Core.BallSort.Solvers;

namespace PuzzleSolver.Benchmarks.SolverBenchmarks;

[WarmupCount(3)]
[MemoryDiagnoser]
[IterationCount(12)]
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
public class BallSortSolverBenchmarks
{
    private ServiceProvider _serviceProvider = null!;
    
    private IBallSortSolver _beamSearchSolver = null!;
    private IBallSortSolver _astarSolver = null!;
    private IBallSortSolver _bfsSolver = null!;
    private IBallSortSolver _dfsSolver = null!;

    [ParamsSource(nameof(States))]
    public BallSortState InitialState { get; set; } = null!;

    public IEnumerable<BallSortState> States()
    {
        yield return new BallSortState(false, 5, 4, [[1,0,0,0],[3,1,1,1],[0,2,2,2],[2,3,3,3],[]]);
        yield return new BallSortState(false, 7, 4, [[3,0,1,4],[0,2,0,4],[],[2,3,3,0],[],[2,3,2,1],[1,4,1,4]]);
        yield return new BallSortState(false, 7, 4, [[],[0,3],[0,4,2],[3,1,3],[0,2,0,1],[2,4,2,3],[4,1,4,1]]);
    }
    
    [GlobalSetup]
    public void Setup()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .Build();
        
        _serviceProvider = new ServiceCollection()
            .AddCoreServices(configuration)
            .BuildServiceProvider();
        
        _beamSearchSolver = _serviceProvider.GetRequiredKeyedService<IBallSortSolver>(BallSortAlgorithm.BeamSearch);
        _astarSolver = _serviceProvider.GetRequiredKeyedService<IBallSortSolver>(BallSortAlgorithm.AStar);
        _bfsSolver = _serviceProvider.GetRequiredKeyedService<IBallSortSolver>(BallSortAlgorithm.Bfs);
        _dfsSolver = _serviceProvider.GetRequiredKeyedService<IBallSortSolver>(BallSortAlgorithm.Dfs);
    }

    [GlobalCleanup]
    public void Cleanup()
        => _serviceProvider.Dispose();

    [Benchmark(Baseline = true)]
    public void Solve_BeamSearch()
        => _beamSearchSolver.Solve(InitialState).ToList();
    
    [Benchmark]
    public void Solve_AStar()
        => _astarSolver.Solve(InitialState).ToList();
    
    [Benchmark]
    public void Solve_Bfs()
        => _bfsSolver.Solve(InitialState).ToList();
    
    [Benchmark]
    public void Solve_Dfs()
        => _dfsSolver.Solve(InitialState).ToList();
}
