using CryptoExchange.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoExchange.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderBookController(BitstampService service) : ControllerBase
{

    [HttpGet("{baseCurrencyCode}/{quoteCurrencyCode}")]
    public async Task<IActionResult> Get(string baseCurrencyCode = "btc", string quoteCurrencyCode = "eur")
    {
        var response = await service.GetOrderBookAsync(baseCurrencyCode, quoteCurrencyCode);
        if (response is null)
            NotFound();
        return Ok(response);
    }

    [HttpGet("GetAvaliableTimeStamps")]
    public async Task<IActionResult> GetAvaliableTimeStamps()
    {
        var response = await service.GetAvaliableTimeStamps();
        if (response is null)
            NotFound();
        return Ok(response);
    }

    [HttpGet("GetOrderBookById/{id}")]
    public async Task<IActionResult> GetOrderBookById(long id)
    {
        var response = await service.GetOrderBookById(id);
        if (response is null)
            NotFound();
        return Ok(response);
    }
}
