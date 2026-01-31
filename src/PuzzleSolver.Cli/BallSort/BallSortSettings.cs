using System.ComponentModel;
using PuzzleSolver.Automation.AutomationFactories;
using PuzzleSolver.Automation.Automations.BallSort;
using PuzzleSolver.Core.BallSort;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.BallSort;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class BallSortSettings : CommandSettings
{
    [CommandArgument(0, "<url>")]
    [Description("Url of the Ball Sort puzzle to solve. Example: https://en.grandgames.net/ballsort_classic/id477125")]
    public string Url { get; init; } = string.Empty;

    [CommandOption("-a|--algorithm")]
    [Description("Solving algorithm to use (BeamSearch, AStar, BFS)")]
    [DefaultValue(BallSortAlgorithm.BeamSearch)]
    public BallSortAlgorithm Algorithm { get; init; } =  BallSortAlgorithm.BeamSearch;

    [CommandOption("-b|--beamwidth")]
    [Description("Beam Width for algorithm")]
    [DefaultValue(BallSortOptions.DefaultBeamWidth)]
    public int BeamWidth { get; init; } = BallSortOptions.DefaultBeamWidth;
    
    [CommandOption("-H|--headless")]
    [Description("Run browser in headless mode")]
    [DefaultValue(AutomationFactoryOptions.DefaultHeadless)]
    public bool Headless { get; init; } = AutomationFactoryOptions.DefaultHeadless;
    
    [CommandOption("-m|--movedelayms")]
    [Description("Delay in milliseconds between moves during playback")]
    [DefaultValue(BallSortAutomationOptions.DefaultMoveDelayMs)]
    public int MoveDelayMs { get; init; } = BallSortAutomationOptions.DefaultMoveDelayMs;
}
