using CryptoExchange.Base.Models;

namespace CryptoExchange.Base.Interfaces;

public interface IBitstampService
{
    Task<OrderBookRecord?> GetOrderBookAsync(string baseCurrencyCode, string quoteCurrencyCode);
}
