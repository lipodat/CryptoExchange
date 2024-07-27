using CryptoExchange.Base.Interfaces;
using CryptoExchange.Base.Models;
using CryptoExchange.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoExchange.Server.Services;

public class BitstampAuditService(IDbContextFactory<CryptoExchangeDbContext> dbContextFactory) : IBitstampAuditService
{
    private readonly IDbContextFactory<CryptoExchangeDbContext> _dbContextFactory = dbContextFactory;
    public async Task SaveOrderBook(OrderBookRecord orderBook, CancellationToken token = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(token);
        context.Add(new OrderBook(orderBook));
        await context.SaveChangesAsync(token);
    }
}
