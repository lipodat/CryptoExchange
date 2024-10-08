﻿using CryptoExchange.Server.Entities;
using CryptoExchange.Base.Interfaces;
using CryptoExchange.Base.Models;
using Microsoft.EntityFrameworkCore;
using CryptoExchange.Server.Entities.Dto;

namespace CryptoExchange.Server.Services;

public class BitstampService(IConfiguration configuration, HttpClient httpClient, IDbContextFactory<CryptoExchangeDbContext> dbContextFactory) : IBitstampService
{
    private readonly IDbContextFactory<CryptoExchangeDbContext> _dbContextFactory = dbContextFactory;
    private IConfiguration _configuration = configuration;
    public async Task<Dictionary<long, DateTimeOffset>> GetAvaliableTimeStamps(CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        return await context.OrderBooks.ToDictionaryAsync(x => x.Id, x => x.TimeStamp, token);
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
        try
        {
            var data = await httpClient.GetFromJsonAsync<BitstampOrderBook>(url);

            if (data is null)
                return null;

            var orderBook = data.ToOrderBook();

            await SaveOrderBook(orderBook);
            return orderBook.ToDto();
        }
        catch (Exception)
        {
            // Here should be some log message.
            return null;
        }
    }
    private async Task SaveOrderBook(OrderBook orderBook, CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        context.Add(orderBook);
        await context.SaveChangesAsync(token);
    }
    private string GetServiceUrl(string baseCurrencyCode, string quoteCurrencyCode) =>
        $"{_configuration.GetSection("BitstampServiceUrl")?.Get<string>()}{baseCurrencyCode}{quoteCurrencyCode}";
}
