using CryptoExchange.Server.Entities;
using CryptoExchange.Base.Interfaces;
using Newtonsoft.Json;
using System.Globalization;
using CryptoExchange.Base.Models;
using CryptoExchange.Base;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchange.Server.Services;

public class BitstampService(HttpClient httpClient, IDbContextFactory<CryptoExchangeDbContext> dbContextFactory) : IBitstampService
{
    private readonly IDbContextFactory<CryptoExchangeDbContext> _dbContextFactory = dbContextFactory;
    
    public async Task<Dictionary<long, DateTimeOffset>> GetAvaliableTimeStamps(CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        return await context.OrderBooks.ToDictionaryAsync(x => x.Id, x=> x.TimeStamp, token);
    }

    public async Task<OrderBookDto?> GetOrderBookById(long id, CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        var orderBook = await context.OrderBooks.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, token);
        return orderBook?.ToDto();
    }

    public async Task<OrderBookDto?> GetOrderBookAsync(string baseCurrencyCode, string quoteCurrencyCode)
    {
        OrderBook result = new();
        var url = GetServiceUrl(baseCurrencyCode, quoteCurrencyCode);

        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        string responseString = await response.Content.ReadAsStringAsync();

        result = FillOrderBookFromBitmapResponse(result, responseString);

        await SaveOrderBook(result);
        return result.ToDto();
    }
    private async Task SaveOrderBook(OrderBook orderBook, CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        context.Add(orderBook);
        await context.SaveChangesAsync(token);
    }
    private static string GetServiceUrl(string baseCurrencyCode, string quoteCurrencyCode) =>
        $"{Constants.BitstampServiceUrl}{baseCurrencyCode}{quoteCurrencyCode}";
    private static OrderBook FillOrderBookFromBitmapResponse(OrderBook orderBook, string response)
    {
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response) ?? [];

        if (data.TryGetValue("timestamp", out object? boxedTimestamp) && long.TryParse(boxedTimestamp.ToString(), out long longTimestamp))
            orderBook.TimeStamp = DateTimeOffset.FromUnixTimeSeconds(longTimestamp);

        orderBook.Items = [];
        if (data.TryGetValue("bids", out object? boxedBids))
            orderBook.Items.AddRange(ParseOrderBookItems(boxedBids, true));

        if (data.TryGetValue("asks", out object? boxedAsks))
            orderBook.Items.AddRange(ParseOrderBookItems(boxedAsks, false));
        return orderBook;
    }
    private static List<OrderBookItem> ParseOrderBookItems(object entries, bool isBid)
    {
        var entriesString = entries.ToString();
        if (string.IsNullOrEmpty(entriesString))
            return [];

        var boxedList = JsonConvert.DeserializeObject<object[]>($"{entriesString}");
        if (boxedList is null)
            return [];

        var result = new List<OrderBookItem>();

        foreach (var entry in boxedList)
        {
            var innerArray = JsonConvert.DeserializeObject<object[]>($"{entry}");
            result.Add(new()
            {
                IsBid = isBid,
                Price = Convert.ToDouble(innerArray?[0], CultureInfo.InvariantCulture),
                Amount = Convert.ToDouble(innerArray?[1], CultureInfo.InvariantCulture)
            });
        }
        return result;
    }
}
