using CryptoExchange.Base.Models;
using CryptoExchange.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoExchange.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderBookController(BitstampService service) : ControllerBase
{

    [HttpGet("{baseCurrencyCode}/{quoteCurrencyCode}")]
    public async Task<OrderBookDto?> Get(string baseCurrencyCode = "btc", string quoteCurrencyCode = "eur")
    {
        var response = await service.GetOrderBookAsync(baseCurrencyCode, quoteCurrencyCode);
        return response;
    }

    [HttpGet("GetAvaliableTimeStamps")]
    public async Task<Dictionary<long, DateTimeOffset>?> GetAvaliableTimeStamps()
    {
        var response = await service.GetAvaliableTimeStamps();
        return response;
    }

    [HttpGet("GetOrderBookById/{id}")]
    public async Task<OrderBookDto?> GetOrderBookById(long id)
    {
        var response = await service.GetOrderBookById(id);
        return response;
    }
}
