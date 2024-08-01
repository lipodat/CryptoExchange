using System.Globalization;

namespace CryptoExchange.Server.Entities.Dto;

public class BitstampOrderBook
{
    public string Timestamp { get; set; } = string.Empty;
    public IEnumerable<IEnumerable<string>> Asks { get; set; } = [];
    public IEnumerable<IEnumerable<string>> Bids { get; set; } = [];

    public OrderBook ToOrderBook()
    {
        return new()
        {
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(Timestamp)),
            Items = [
                ..Asks.Select(x => new OrderBookItem()
                {
                    Price = Convert.ToDouble(x.First(), CultureInfo.InvariantCulture),
                    Amount = Convert.ToDouble(x.Last(), CultureInfo.InvariantCulture)
                }),
                ..Bids.Select(x => new OrderBookItem()
                {
                    IsBid = true,
                    Price = Convert.ToDouble(x.First(), CultureInfo.InvariantCulture),
                    Amount = Convert.ToDouble(x.Last(), CultureInfo.InvariantCulture)
                }),
            ]
        };
    }
}
