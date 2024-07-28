using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Net.Http;
using System.Timers;

namespace CryptoExchange.WebClient.Pages;

public partial class Home
{
    [Inject] private HttpClient _httpClient { get; set; } = default!;
    private OrderBookRecord _orderBook = new();
    private double? _midPointPrice;
    private System.Timers.Timer _timer;
    private double? _userAmount;
    private double? UserAmount { 
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
            await InvokeAsync(UpdateOrderBookAsync);
        };
        _timer.Enabled = true;
    }
    protected override async Task OnInitializedAsync()
    {
        await UpdateOrderBookAsync();
        CalculateEstimatedBtcPrice();
    }

    private async Task UpdateOrderBookAsync()
    {
        _orderBook = await _httpClient.GetFromJsonAsync<OrderBookRecord?>("OrderBook") ?? new();
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
