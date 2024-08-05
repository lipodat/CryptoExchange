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
    }
}
