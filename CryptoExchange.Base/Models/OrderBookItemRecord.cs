using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoExchange.Base.Models;

public record OrderBookItemRecord
{
    public long Id { get; set; }
    public double Price { get; set; }
    public double Amount { get; set; }
}
