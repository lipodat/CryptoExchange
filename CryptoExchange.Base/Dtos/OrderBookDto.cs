namespace CryptoExchange.Base.Models;

public class OrderBookDto
{
    public long Id { get; set; }
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;
    public List<OrderBookItemDto> Bids { get; set; } = [];
    public List<OrderBookItemDto> Asks { get; set; } = [];
}
