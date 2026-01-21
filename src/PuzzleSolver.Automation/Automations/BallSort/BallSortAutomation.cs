using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Automation.Utils;
using PuzzleSolver.Core.BallSort;

namespace PuzzleSolver.Automation.Automations.BallSort;

public sealed class BallSortAutomation : BaseAutomation<BallSortState, BallSortMove, BallSortOptions>, IDisposable
{
    private const string PlayBtnSelector = "#timeradd_but";
    private const string FrameSelector = "#ggPuzzleFrame";
    private const string SettingsBtnSelector = ".gameButton[alt='Settings']";
    private const string AutoMoveBtnSelector = ".gameButton[alt='Auto Moves']";
    private const string ScriptSelector = "#main_game_div > script";
    private const string TubeStartSelector = "#tube_";
    
    private const string FlasksCountGroup = "sx";
    private const string FlaskCapacityGroup = "sy";
    private const string LayoutGroup = "p";
    private static readonly Regex InitialStateRegex = new(
        @$"{FlasksCountGroup}:\s*(?<{FlasksCountGroup}>\d+),\s*" +
        @$"{FlaskCapacityGroup}:\s*(?<{FlaskCapacityGroup}>\d+),.*?" +
        @$"{LayoutGroup}:\s*'(?<{LayoutGroup}>\[.*?\])'",
        RegexOptions.Singleline | RegexOptions.Compiled
    );

    private volatile BallSortAutomationOptions _options;
    private readonly IDisposable? _optionsSubscription;
    
    public BallSortAutomation(IPage page, IOptionsMonitor<BallSortAutomationOptions> optionsMonitor) : base(page)
    {
        _options = optionsMonitor.CurrentValue;
        _optionsSubscription = optionsMonitor.OnChange(value => _options = value);
    }

    protected override string UrlStart => "https://grandgames.net/ballsort";

    public override async Task ConfigureAsync()
    {
        await base.ConfigureAsync();
        await _page.ClickAsync(PlayBtnSelector);
        await _page.FrameLocator(FrameSelector).Locator(SettingsBtnSelector).ClickAsync();
        await _page.FrameLocator(FrameSelector).Locator(AutoMoveBtnSelector).ClickAsync();
        await _page.FrameLocator(FrameSelector).Locator(SettingsBtnSelector).ClickAsync();
    }
    
    public override async Task<BallSortState> GetInitialStateAsync()
    {
        string scriptContent = await _page.InnerTextAsync(ScriptSelector);

        Match match = InitialStateRegex.Match(scriptContent);
        if (!match.Success)
            throw new InitialStateNotFoundException("Failed to extract initial state.");

        if (!int.TryParse(match.Groups[FlasksCountGroup].Value, out int flasksCount))
            throw new InitialStateNotFoundException($"Invalid flasks count: {match.Groups[FlasksCountGroup].Value}");

        if (!int.TryParse(match.Groups[FlaskCapacityGroup].Value, out int flaskCapacity))
            throw new InitialStateNotFoundException($"Invalid flask capacity: {match.Groups[FlaskCapacityGroup].Value}");

        string layoutString = match.Groups[LayoutGroup].Value;
        var serializerOptions = new JsonSerializerOptions
        {
            Converters = { new ByteContextConverter() }
        };

        byte[][] layout = JsonSerializer.Deserialize<byte[][]>(layoutString, serializerOptions)
            ?? throw new InitialStateNotFoundException("Layout is empty.");

        return new(flasksCount, flaskCapacity, layout);
    }

    public override async Task ApplyMovesAsync(IEnumerable<BallSortMove> moves, CancellationToken cancellationToken = default)
    {
        foreach (BallSortMove move in moves)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException(nameof(ApplyMovesAsync));

            await _page.FrameLocator(FrameSelector).Locator(TubeStartSelector + move.From).ClickAsync();
            await Task.Delay(_options.MoveDelayMs, cancellationToken);
            
            await _page.FrameLocator(FrameSelector).Locator(TubeStartSelector + move.To).ClickAsync();
            await Task.Delay(_options.MoveDelayMs, cancellationToken);
        }
    }

    public void Dispose()
        => _optionsSubscription?.Dispose();
}
