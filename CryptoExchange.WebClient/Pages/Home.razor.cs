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
    private System.Timers.Timer _timer;
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
    }

    private async Task UpdateOrderBookAsync()
    {
        _orderBook = await _httpClient.GetFromJsonAsync<OrderBookRecord?>("OrderBook") ?? new();
        StateHasChanged();
    }
}
