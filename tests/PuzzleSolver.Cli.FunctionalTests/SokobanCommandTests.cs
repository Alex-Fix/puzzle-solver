using FluentAssertions;
using PuzzleSolver.Cli.Sokoban;
using PuzzleSolver.Cli.Utils;
using PuzzleSolver.Core.Sokoban;
using Spectre.Console.Testing;

namespace PuzzleSolver.Cli.FunctionalTests;

public sealed class SokobanCommandTests : IClassFixture<CommandAppFixture>
{
    private readonly CommandAppFixture _commandAppFixture;

    public SokobanCommandTests(CommandAppFixture commandAppFixture)
    {
        _commandAppFixture = commandAppFixture;
        _commandAppFixture.App.SetDefaultCommand<SokobanCommand>();
    }
        
    [Fact]
    public async Task Should_ParseArguments()
    {
        const string url = "https://en.grandgames.net/sokoban/id164874";
        const SokobanAlgorithm algorithm = SokobanAlgorithm.AStar;
        const int moveDelayMs = 2;
        const bool headless = true;
        
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        
        CommandAppResult result = await _commandAppFixture.App.RunAsync(
        [
            url, 
            "-a", algorithm.ToString(), 
            "-m", moveDelayMs.ToString(), 
            "-H", headless.ToString()
        ], cts.Token);

        SokobanSettings? settings = result.Settings as SokobanSettings;
        
        settings.Should().NotBeNull();
        settings.Url.Should().Be(url);
        settings.Algorithm.Should().Be(algorithm);
        settings.MoveDelayMs.Should().Be(moveDelayMs);
        settings.Headless.Should().Be(headless);
    }

    [Fact]
    public async Task ShouldExit_Success()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        
        CommandAppResult result = await _commandAppFixture.App.RunAsync(["https://en.grandgames.net/sokoban/id164874", "-H", "true"], cts.Token);
        
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
        
        CommandAppResult result = await _commandAppFixture.App.RunAsync(["https://en.grandgames.net/sokoban/id164874", "-H", "true"], cts.Token);
        
        result.ExitCode.Should().Be(ExitCodes.Cancelled);
    }

    // Difficult to produce
    // [Fact]
    // public async Task ShouldExit_SolutionNotFound() { }

    // Difficult to produce
    // [Fact]
    // public async Task ShouldExit_InitialStateNotFound() { }

    // Difficult to produce
    // [Fact]
    // public async Task ShouldExit_Unknown() { }
}
