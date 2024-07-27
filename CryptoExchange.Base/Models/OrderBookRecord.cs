namespace CryptoExchange.Base.Models;

public record OrderBookRecord
{
    public long Id { get; set; }
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;
    public List<OrderBookItemRecord> Bids { get; set; } = [];
    public List<OrderBookItemRecord> Asks { get; set; } = [];
}
