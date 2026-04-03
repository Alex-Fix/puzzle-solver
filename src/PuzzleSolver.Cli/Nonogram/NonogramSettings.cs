using System.ComponentModel;
using PuzzleSolver.Automation.AutomationFactories;
using PuzzleSolver.Automation.Automations.Nonogram;
using PuzzleSolver.Core.Nonogram;
using Spectre.Console.Cli;

namespace PuzzleSolver.Cli.Nonogram;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class NonogramSettings : CommandSettings
{
    [CommandArgument(0, "<url>")]
    [Description("Url of the Nonogram puzzle to solve. Example: https://www.nonograms.ru/nonograms/i/80309")]
    public string Url { get; init; } = string.Empty;

    [CommandOption("-a|--algorithm")]
    [Description("Solving algorithm to use (Smt, AStar)")]
    [DefaultValue(NonogramAlgorithm.Smt)]
    public NonogramAlgorithm Algorithm { get; init; } =  NonogramAlgorithm.Smt;

    [CommandOption("-H|--headless")]
    [Description("Run browser in headless mode")]
    [DefaultValue(AutomationFactoryOptions.DefaultHeadless)]
    public bool Headless { get; init; } = AutomationFactoryOptions.DefaultHeadless;
    
    [CommandOption("-m|--movedelayms")]
    [Description("Delay in milliseconds between moves during playback")]
    [DefaultValue(NonogramAutomationOptions.DefaultMoveDelayMs)]
    public int MoveDelayMs { get; init; } = NonogramAutomationOptions.DefaultMoveDelayMs;
}
