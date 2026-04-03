using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PuzzleSolver.Automation.Exceptions;
using PuzzleSolver.Core.Nonogram;

namespace PuzzleSolver.Automation.Automations.Nonogram;

public sealed class NonogramAutomation : BaseAutomation<NonogramState, NonogramMove, NonogramOptions>, IDisposable
{
    private const string RowCluesSelector = ".nmtl tr";
    private const string CellSelector = "td";
    private const string ColumnCluesSelector = ".nmtt tr";
    private const string CellFormatSelector = "#nmf{0}_{1}";
    
    private volatile NonogramAutomationOptions _options;
    private readonly IDisposable? _optionsSubscription;
    
    public NonogramAutomation(IPage page, IOptionsMonitor<NonogramAutomationOptions> optionsMonitor) : base(page)
    {
        _options = optionsMonitor.CurrentValue;
        _optionsSubscription = optionsMonitor.OnChange(value => _options = value);
    }

    protected override string UrlStart => "https://www.nonograms.ru/nonograms/i/";

    public override async Task NavigateAsync(string url)
    {
        if (!url.StartsWith(UrlStart))
            throw new UrlMismatchException(UrlStart);

        await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
    }

    public override Task ConfigureAsync()
        => Task.CompletedTask;

    public override async Task<NonogramState> GetInitialStateAsync()
    {
        List<IReadOnlyList<string>> rowData = await GetClueMatrixAsync(RowCluesSelector);
        List<IReadOnlyList<string>> columnData = await GetClueMatrixAsync(ColumnCluesSelector);

        return new NonogramState(
            ParseRows(rowData),
            ParseAndTranspose(columnData)
        );
    }

    private async Task<List<IReadOnlyList<string>>> GetClueMatrixAsync(string selector)
    {
        IReadOnlyList<ILocator> rows = await _page.Locator(selector).AllAsync();
        var matrix = new List<IReadOnlyList<string>>();

        foreach (ILocator row in rows)
        {
            matrix.Add(await row.Locator(CellSelector).AllInnerTextsAsync());
        }
        
        return matrix;
    }

    private static byte[][] ParseRows(List<IReadOnlyList<string>> matrix)
    {
        return matrix.Select(row => row
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => byte.Parse(s.Trim()))
            .ToArray()
        ).ToArray();
    }

    private static byte[][] ParseAndTranspose(List<IReadOnlyList<string>> matrix)
    {
        if (matrix.Count == 0) 
            return [];

        int columnCount = matrix[0].Count;
        var columnClues = new byte[columnCount][];

        for (int column = 0; column < columnCount; ++column)
        {
            var clue = new List<byte>();
            foreach (IReadOnlyList<string> row in matrix)
            {
                string text = row[column];
                if (string.IsNullOrWhiteSpace(text))
                    continue;
                
                clue.Add(byte.Parse(text.Trim()));
            }
            
            columnClues[column] = clue.ToArray();
        }

        return columnClues;
    }
    
    public override async Task ApplyMovesAsync(IEnumerable<NonogramMove> moves, CancellationToken cancellationToken = default)
    {
        foreach (var move in moves)
        {
            if (!move.Filled) continue;

            string cellSelector = string.Format(CellFormatSelector, move.Column, move.Row);
            await _page.Locator(cellSelector).ClickAsync();
            await Task.Delay(_options.MoveDelayMs, cancellationToken);
        }
    }

    public void Dispose()
        => _optionsSubscription?.Dispose();
}
