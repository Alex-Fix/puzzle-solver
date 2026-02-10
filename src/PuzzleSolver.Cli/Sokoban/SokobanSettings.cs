using System.ComponentModel;
using PuzzleSolver.Automation.AutomationFactories;
using PuzzleSolver.Automation.Automations.Sokoban;
using PuzzleSolver.Core.Sokoban;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.Sokoban;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SokobanSettings: CommandSettings
{
    [CommandArgument(0, "<url>")]
    [Description("Url of the Sokoban puzzle to solve. Example: https://en.grandgames.net/sokoban/id195119")]
    public string Url { get; init; } = string.Empty;

    [CommandOption("-a|--algorithm")]
    [Description("Solving algorithm to use (AStar)")]
    [DefaultValue(SokobanAlgorithm.AStar)]
    public SokobanAlgorithm Algorithm { get; init; } =  SokobanAlgorithm.AStar;

    [CommandOption("-H|--headless")]
    [Description("Run browser in headless mode")]
    [DefaultValue(AutomationFactoryOptions.DefaultHeadless)]
    public bool Headless { get; init; } = AutomationFactoryOptions.DefaultHeadless;
    
    [CommandOption("-m|--movedelayms")]
    [Description("Delay in milliseconds between moves during playback")]
    [DefaultValue(SokobanAutomationOptions.DefaultMoveDelayMs)]
    public int MoveDelayMs { get; init; } = SokobanAutomationOptions.DefaultMoveDelayMs;
}
