using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Timers;
using BlazorBootstrap;

namespace CryptoExchange.WebClient.Pages;

public partial class Home
{
    [Inject] private HttpClient _httpClient { get; set; } = default!;
    private OrderBookDto _orderBook = new();
    private double? _midPointPrice;
    private System.Timers.Timer _timer;
    private double? _userAmount;
    private BarChart barChart = default!;
    private double? UserAmount
    {
        get
        {
            return _userAmount;
        }
        set
        {
            _userAmount = value;
            CalculateUserBtcPrice();
        }
    }
    private double? _userPrice;
    public Home()
    {
        _timer = new()
        {
            Interval = 10000
        };
        _timer.Elapsed += async (object? sender, ElapsedEventArgs e) =>
        {
            await InvokeAsync(async () => await UpdateOrderBookAsync());
        };
        _timer.Enabled = true;
    }
    protected override async Task OnInitializedAsync()
    {
        await UpdateOrderBookAsync(true);
        CalculateEstimatedBtcPrice();
    }
    private async Task RenderChartAsync(bool firstRun = false)
    {
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


        if (firstRun)
            await barChart.InitializeAsync(data, options);
        else
            await barChart.UpdateAsync(data, options);
    }

    private async Task UpdateOrderBookAsync(bool firstRun = false)
    {
        _orderBook = await _httpClient.GetFromJsonAsync<OrderBookDto?>("OrderBook/btc/eur") ?? new();
        await RenderChartAsync(firstRun);
        CalculateEstimatedBtcPrice();
        StateHasChanged();
    }

    public void CalculateEstimatedBtcPrice()
    {
        if (_orderBook is null || _orderBook.Bids?.Count == 0 || _orderBook.Asks?.Count == 0)
            return;

        // Calculate weighted average price
        double totalPriceBids = _orderBook.Bids.OrderByDescending(x => x.Price).Take(10).Sum(bid => bid.Price * bid.Amount);
        double totalVolumeBids = _orderBook.Bids.OrderByDescending(x => x.Price).Take(10).Sum(bid => bid.Amount);

        double totalPriceAsks = _orderBook.Asks.OrderBy(x => x.Price).Take(10).Sum(ask => ask.Price * ask.Amount);
        double totalVolumeAsks = _orderBook.Asks.OrderBy(x => x.Price).Take(10).Sum(ask => ask.Amount);

        _midPointPrice = (totalPriceBids + totalPriceAsks) / (totalVolumeBids + totalVolumeAsks);
        CalculateUserBtcPrice();
    }

    public void CalculateUserBtcPrice()
    {
        if (UserAmount is null || _midPointPrice is null)
            return;
        _userPrice = UserAmount * _midPointPrice;
    }
}
