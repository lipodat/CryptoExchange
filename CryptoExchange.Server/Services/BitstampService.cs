using CryptoExchange.Server.Entities;
using CryptoExchange.Base.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using CryptoExchange.Base.Models;
using CryptoExchange.Base;

namespace CryptoExchange.Server.Services;

public class BitstampService(HttpClient httpClient) : IBitstampService
{
    public async Task<ActionResult<OrderBookRecord>> GetOrderBookAsync(string baseCurrencyCode, string quoteCurrencyCode)
    {
        OrderBook result = new();
        var url = GetServiceUrl(baseCurrencyCode, quoteCurrencyCode);

        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return result.ToRecord();

        string responseString = await response.Content.ReadAsStringAsync();

        FillOrderBookFromBitmapResponse(ref result, responseString);

        return result.ToRecord();
    }

    private static string GetServiceUrl(string baseCurrencyCode, string quoteCurrencyCode) =>
        $"{Constants.BitstampServiceUrl}{baseCurrencyCode}{quoteCurrencyCode}";
    private static void FillOrderBookFromBitmapResponse(ref OrderBook orderBook, string response)
    {
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response) ?? [];

        if (data.TryGetValue("timestamp", out object? boxedTimestamp) && long.TryParse(boxedTimestamp.ToString(), out long longTimestamp))
            orderBook.TimeStamp = DateTimeOffset.FromUnixTimeSeconds(longTimestamp);

        orderBook.Items = [];
        if (data.TryGetValue("bids", out object? boxedBids))
            orderBook.Items.AddRange(ParseOrderBookItems(boxedBids, true));

        if (data.TryGetValue("asks", out object? boxedAsks))
            orderBook.Items.AddRange(ParseOrderBookItems(boxedAsks, false));
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
