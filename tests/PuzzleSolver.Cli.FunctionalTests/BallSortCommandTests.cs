using FluentAssertions;
using PuzzleSolver.Cli.BallSort;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core.BallSort;
using Spectre.Console.Testing;

namespace PuzzleSolver.Cli.FunctionalTests;

public sealed class BallSortCommandTests : IClassFixture<CommandAppFixture>
{
    private readonly CommandAppFixture _commandAppFixture;

    public BallSortCommandTests(CommandAppFixture commandAppFixture)
    {
        _commandAppFixture = commandAppFixture;
        _commandAppFixture.App.SetDefaultCommand<BallSortCommand>();
    }
        
    [Fact]
    public async Task Should_ParseArguments()
    {
        const string url = "https://en.grandgames.net/ballsort_classic/id564720";
        const BallSortAlgorithm algorithm = BallSortAlgorithm.BeamSearch;
        const int beamWidth = 1;
        const int moveDelayMs = 2;
        const bool headless = true;
        
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        CommandAppResult result = await _commandAppFixture.App.RunAsync(
        [
            url, 
            "-a", algorithm.ToString(), 
            "-b", beamWidth.ToString(), 
            "-m", moveDelayMs.ToString(), 
            "-H", headless.ToString()
        ], cts.Token);

        BallSortSettings? settings = result.Settings as BallSortSettings;
        
        settings.Should().NotBeNull();
        settings.Url.Should().Be(url);
        settings.Algorithm.Should().Be(algorithm);
        settings.BeamWidth.Should().Be(beamWidth);
        settings.MoveDelayMs.Should().Be(moveDelayMs);
        settings.Headless.Should().Be(headless);
    }

    [Fact]
    public async Task ShouldExit_Success()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        
        CommandAppResult result = await _commandAppFixture.App.RunAsync(["https://en.grandgames.net/ballsort_classic/id381328", "-H", "true"], cts.Token);
        
        result.ExitCode.Should().Be(ExitCodes.Success);
    }
    
    [Fact]
    public async Task ShouldExit_UrlMismatch()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        CommandAppResult result = await _commandAppFixture.App.RunAsync(["https://google.com", "-H", "true"], cts.Token);
        
        result.ExitCode.Should().Be(ExitCodes.UrlMismatch);
    }

    [Fact]
    public async Task ShouldExit_Cancelled()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        
        CommandAppResult result = await _commandAppFixture.App.RunAsync(["https://en.grandgames.net/ballsort_classic/id564720", "-H", "true"], cts.Token);
        
        result.ExitCode.Should().Be(ExitCodes.Cancelled);
    }

    [Fact]
    public async Task ShouldExit_SolutionNotFound()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        CommandAppResult result = await _commandAppFixture.App.RunAsync(["https://en.grandgames.net/ballsort_classic/id564720", "-b", "1", "-H", "true"], cts.Token);
        
        result.ExitCode.Should().Be(ExitCodes.SolutionNotFound);
    }

    // Difficult to produce
    // [Fact]
    // public async Task ShouldExit_InitialStateNotFound() { }

    // Difficult to produce
    // [Fact]
    // public async Task ShouldExit_Unknown() { }
}
