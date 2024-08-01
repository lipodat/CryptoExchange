using CryptoExchange.Server.Entities;
using CryptoExchange.Base.Interfaces;
using Newtonsoft.Json;
using System.Globalization;
using CryptoExchange.Base.Models;
using CryptoExchange.Base;
using Microsoft.EntityFrameworkCore;
using CryptoExchange.Server.Entities.Dto;

namespace CryptoExchange.Server.Services;

public class BitstampService(HttpClient httpClient, IDbContextFactory<CryptoExchangeDbContext> dbContextFactory) : IBitstampService
{
    private readonly IDbContextFactory<CryptoExchangeDbContext> _dbContextFactory = dbContextFactory;
    
    public async Task<Dictionary<long, DateTimeOffset>> GetAvaliableTimeStamps(CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        return await context.OrderBooks.ToDictionaryAsync(x => x.Id, x=> x.TimeStamp, token);
    }

    public async Task<OrderBookDto?> GetOrderBookById(long id, CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        var orderBook = await context.OrderBooks.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, token);
        return orderBook?.ToDto();
    }

    public async Task<OrderBookDto?> GetOrderBookAsync(string baseCurrencyCode, string quoteCurrencyCode)
    {
        var url = GetServiceUrl(baseCurrencyCode, quoteCurrencyCode);

        var data = await httpClient.GetFromJsonAsync<BitstampOrderBook>(url);

        if (data is null)
            return null;

        var orderBook = data.ToOrderBook();

        await SaveOrderBook(orderBook);
        return orderBook.ToDto();
    }
    private async Task SaveOrderBook(OrderBook orderBook, CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        context.Add(orderBook);
        await context.SaveChangesAsync(token);
    }
    private static string GetServiceUrl(string baseCurrencyCode, string quoteCurrencyCode) =>
        $"{Constants.BitstampServiceUrl}{baseCurrencyCode}{quoteCurrencyCode}";
}
