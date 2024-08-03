using CryptoExchange.Base.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoExchange.Server.Entities;

public class OrderBookItem
{
    public long Id { get; set; }
    public bool IsBid { get; set; }
    public double Price { get; set; }
    public double Amount { get; set; }

    [ForeignKey("OrderBookId")] 
    public OrderBook OrderBook { get; set; } = default!;
    public OrderBookItemDto ToDto() => new() { Id = Id, Price = Price, Amount = Amount };
}
