using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Net.Http;

namespace CryptoExchange.WebClient.Pages;

public partial class Home
{
    [Inject] private HttpClient _httpClient { get; set; } = default!;
    private OrderBookRecord _orderBook = new();
    protected override async Task OnInitializedAsync()
    {
        _orderBook = await _httpClient.GetFromJsonAsync<OrderBookRecord?>("OrderBook") ?? new();
    }
}
