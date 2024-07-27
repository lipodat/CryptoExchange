using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.Mvc;

namespace CryptoExchange.Base.Interfaces;

public interface IBitstampService
{
    Task<ActionResult<OrderBookRecord>> GetOrderBookAsync(string baseCurrencyCode, string quoteCurrencyCode);
}
