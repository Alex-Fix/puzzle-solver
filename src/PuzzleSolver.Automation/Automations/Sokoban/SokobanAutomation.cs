using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Core.Sokoban;

namespace PuzzleSolver.Automation.Automations.Sokoban;

public sealed class SokobanAutomation : BaseAutomation<SokobanState, SokobanMove, SokobanOptions>, IDisposable
{
    private const string PlayBtnSelector = "#timeradd_but";
    private const string FrameSelector = "#ggPuzzleFrame";
    private const string GameContainerSelector = "#main_game_div";
    private const string ScriptSelector = GameContainerSelector + " > script";
    private const string CanvasSelector = "#gamediv > canvas";
    
    private const string WidthGroup = "sx";
    private const string HeightGroup = "sy";
    private const string LayoutGroup = "p";
    private static readonly Regex InitialStateRegex = new Regex(
        $@"{LayoutGroup}:\s*'(?<{LayoutGroup}>[^']*)',\s*" + 
        $@"{WidthGroup}:\s*(?<{WidthGroup}>\d+),\s*" +
        $@"{HeightGroup}:\s*(?<{HeightGroup}>\d+),",
        RegexOptions.Singleline | RegexOptions.Compiled
    );

    private volatile SokobanAutomationOptions _options;
    private readonly IDisposable? _optionsSubscription;
    
    public SokobanAutomation(IPage page, IOptionsMonitor<SokobanAutomationOptions> optionsMonitor) : base(page)
    {
        _options = optionsMonitor.CurrentValue;
        _optionsSubscription = optionsMonitor.OnChange(value => _options = value);
    }

    protected override string UrlStart => "https://en.grandgames.net/sokoban";

    public override async Task ConfigureAsync()
    {
        await base.ConfigureAsync();
        await _page.ClickAsync(PlayBtnSelector);
        await _page.Locator(GameContainerSelector).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }
    
    public override async Task<SokobanState> GetInitialStateAsync()
    {
        string scriptContent = await _page.InnerTextAsync(ScriptSelector);

        Match match = InitialStateRegex.Match(scriptContent);
        if (!match.Success)
            throw new InitialStateNotFoundException("Failed to extract initial state.");

        string layout = match.Groups[LayoutGroup].Value;
        
        if (!int.TryParse(match.Groups[WidthGroup].Value, out int width))
            throw new InitialStateNotFoundException($"Invalid width: {match.Groups[WidthGroup].Value}");

        if (!int.TryParse(match.Groups[HeightGroup].Value, out int height))
            throw new InitialStateNotFoundException($"Invalid height: {match.Groups[HeightGroup].Value}");

        return new(height, width, layout);
    }

    public override async Task ApplyMovesAsync(IEnumerable<SokobanMove> moves, CancellationToken cancellationToken = default)
    {
        foreach (var move in moves)
        {
            cancellationToken.ThrowIfCancellationRequested();

            (int dx, int dy) = move.Direction switch
            {
                SokobanMoveDirection.Up => (0, -1),
                SokobanMoveDirection.Right => (1, 0),
                SokobanMoveDirection.Down => (0, 1),
                SokobanMoveDirection.Left => (-1, 0),
                _ => throw new ArgumentException($"Unknown: {move.Direction}")
            };

            await _page.FrameLocator(FrameSelector).Locator(CanvasSelector).EvaluateAsync("(el, [dx, dy]) => TryKeyMove(dx, dy)", new object[] { dx, dy });
            await Task.Delay(_options.MoveDelayMs, cancellationToken);
        }
    }
    
    public void Dispose()
        => _optionsSubscription?.Dispose();
}
