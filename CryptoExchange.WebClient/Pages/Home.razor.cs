using CryptoExchange.Base;
using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace CryptoExchange.WebClient.Pages;

public partial class Home
{
    [Inject] private HttpClient HttpClient { get; set; } = default!;
    private OrderBookDto _orderBook = new();
    private System.Timers.Timer? _timer;
    private double? _userAmount;
    private readonly int _refreshRateInSeconds = 10;
    private double? UserAmount
    {
        get
        {
            return _userAmount;
        }
        set
        {
            _userAmount = value;
            CalculateUserPrice();
        }
    }
    private double? _userPrice;
    private HubConnection? _hubConnection;

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder().WithUrl(new Uri(HttpClient.BaseAddress!, Constants.SignalR_Path)).Build();
        _hubConnection.On<OrderBookDto>(Constants.SignalR_Method, (orderBook) =>
        {
            _orderBook = orderBook;
            CalculateUserPrice();
            StateHasChanged();
        });
        await _hubConnection.StartAsync();
        /*_timer = new()
        {
            Interval = _refreshRateInSeconds * 1000
        };
        _timer.Elapsed += async (object? sender, ElapsedEventArgs e) =>
        {
            await InvokeAsync(async () => await UpdateOrderBookAsync());
        };
        _timer.Enabled = true;
        await UpdateOrderBookAsync(true);*/
    }

    private async Task UpdateOrderBookAsync(bool firstRun = false)
    {
        _orderBook = await HttpClient.GetFromJsonAsync<OrderBookDto?>("OrderBook/btc/eur") ?? new();
        CalculateUserPrice();
        StateHasChanged();
    }

    public void CalculateUserPrice()
    {
        if (UserAmount is null || _orderBook.Bids.Count == 0)
            return;
        _userPrice = CalculateBuyPrice(_orderBook.Asks.OrderBy(x => x.Price), UserAmount.Value);
    }

    private static double CalculateBuyPrice(IEnumerable<OrderBookItemDto> asks, double desiredAmount)
    {
        double totalSpent = 0;
        double remainingAmount = desiredAmount;

        foreach (var ask in asks)
        {
            var amountToBuy = Math.Min(ask.Amount, remainingAmount);
            totalSpent += amountToBuy * ask.Price;
            remainingAmount -= amountToBuy;

            if (remainingAmount <= 0)
                break;
        }

        return totalSpent;
    }
}