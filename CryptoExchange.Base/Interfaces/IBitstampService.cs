using CryptoExchange.Base.Models;

namespace CryptoExchange.Base.Interfaces;

public interface IBitstampService
{
    Task<Dictionary<long, DateTimeOffset>> GetAvaliableTimeStamps(CancellationToken token = default);
    Task<OrderBookDto?> GetOrderBookById(long id, CancellationToken token = default);
    Task<OrderBookDto?> GetOrderBookAsync(string baseCurrencyCode, string quoteCurrencyCode);
}
