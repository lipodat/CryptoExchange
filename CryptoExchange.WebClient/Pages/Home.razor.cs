using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Timers;

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

    protected override async Task OnInitializedAsync()
    {
        _timer = new()
        {
            Interval = _refreshRateInSeconds * 1000
        };
        _timer.Elapsed += async (object? sender, ElapsedEventArgs e) =>
        {
            await InvokeAsync(async () => await UpdateOrderBookAsync());
        };
        _timer.Enabled = true;
        await UpdateOrderBookAsync(true);
        CalculateUserPrice();
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
        _userPrice = CalculateBuyPriceRecursive(_orderBook.Asks.OrderBy(x => x.Price), UserAmount.Value, 0);
    }

    private static double CalculateBuyPriceRecursive(IEnumerable<OrderBookItemDto> asks, double remainingAmount, double totalSpent)
    {
        var currentAsk = asks.FirstOrDefault();
        if (remainingAmount <= 0 || currentAsk is null)
            return totalSpent;

        var amountToBuy = Math.Min(currentAsk.Amount, remainingAmount);
        var spentOnThisAsk = amountToBuy * currentAsk.Price;

        return CalculateBuyPriceRecursive(asks.Skip(1), remainingAmount - amountToBuy, totalSpent + spentOnThisAsk);
    }
}