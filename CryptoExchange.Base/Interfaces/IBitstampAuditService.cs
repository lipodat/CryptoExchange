using CryptoExchange.Base.Models;

namespace CryptoExchange.Base.Interfaces;

public interface IBitstampAuditService
{
    Task SaveOrderBook(OrderBookRecord orderBook, CancellationToken token = default);
}
