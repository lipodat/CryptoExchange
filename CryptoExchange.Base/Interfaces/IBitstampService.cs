using CryptoExchange.Base.Models;

namespace CryptoExchange.Base.Interfaces;

public interface IBitstampService
{
    Task<OrderBookDto?> GetOrderBookAsync(string baseCurrencyCode, string quoteCurrencyCode);
}
