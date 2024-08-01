using BlazorBootstrap;
using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace CryptoExchange.WebClient.Pages;

public partial class Audit
{
    [Inject] private HttpClient HttpClient { get; set; } = default!;
    private Dictionary<long, DateTimeOffset> _bookTimestamps = [];
    private long? _selectedOrderbookId;
    private OrderBookDto? _orderBook;
    private BarChart barChart = default!;
    private bool _chartInitialized;

    protected override async Task OnInitializedAsync()
    {
        _bookTimestamps = await HttpClient.GetFromJsonAsync<Dictionary<long, DateTimeOffset>?>("OrderBook/GetAvaliableTimeStamps") ?? [];
    }
    private string DisableIfNotSelected() => _selectedOrderbookId is null ? "disabled" : "";
    private async Task LoadOrderBook()
    {

        _orderBook = await HttpClient.GetFromJsonAsync<OrderBookDto?>($"OrderBook/GetOrderBookById/{_selectedOrderbookId}");
        if (_orderBook is null)
            return;
        await RenderChartAsync();
    }
    private async Task RenderChartAsync()
    {
        if (_orderBook is null)
            return;

        var topTenAsks = _orderBook.Asks.OrderBy(x => x.Price).Take(10);
        var firstTenBids = _orderBook.Bids.OrderByDescending(x => x.Price).Take(10).OrderBy(x => x.Price);
        var data = new ChartData
        {
            Labels = [.. firstTenBids.Select(x => x.Price.ToString()), .. topTenAsks.Select(x => x.Price.ToString())],
            Datasets =
                [
                    new BarChartDataset()
                    {
                        Label = "Bids",
                        Data = [ ..firstTenBids.Select(x => x.Amount) ],
                        BackgroundColor = ["rgb(34, 139, 34)"],
                        CategoryPercentage = 0.8,
                        BarPercentage = 1
                    },
                    new BarChartDataset()
                    {
                        Label = "Asks",
                        Data = [.. topTenAsks.Select(_ => 0), .. topTenAsks.Select(x => x.Amount) ],
                        BackgroundColor = ["rgb(178, 34, 34)"],
                        CategoryPercentage = 0.8,
                        BarPercentage = 1
                    }
                ],
        };

        var options = new BarChartOptions();

        options.Interaction.Mode = InteractionMode.Index;

        options.Responsive = true;

        options.Scales.X!.Title = new ChartAxesTitle { Text = "Price", Display = true };
        options.Scales.Y!.Title = new ChartAxesTitle { Text = "Amount", Display = true };
        options.Scales.X!.Stacked = true;


        if (!_chartInitialized)
            await barChart.InitializeAsync(data, options);
        else
            await barChart.UpdateAsync(data, options);

        _chartInitialized = true;
    }
}
