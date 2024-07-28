using CryptoExchange.Base.Models;
using CryptoExchange.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoExchange.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderBookController(BitstampService service) : ControllerBase
{

    [HttpGet(Name = "GetBtcOrderBook")]
    public async Task<OrderBookRecord?> Get()
    {
        var response = await service.GetOrderBookAsync("btc", "eur");
        return response;
    }
}
