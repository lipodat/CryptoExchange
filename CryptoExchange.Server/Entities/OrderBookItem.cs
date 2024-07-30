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

    public OrderBookItem()
    {

    }

    public OrderBookItem(OrderBookItemDto record, bool isBid, OrderBook orderBook)
    {
        Id = record.Id;
        IsBid = isBid;
        Price = record.Price;
        Amount = record.Amount;
        OrderBook = orderBook;
    }
    public OrderBookItemDto ToDto() => new() { Id = Id, Price = Price, Amount = Amount };
}
