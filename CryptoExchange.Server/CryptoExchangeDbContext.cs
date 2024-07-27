using Microsoft.EntityFrameworkCore;
using CryptoExchange.Server.Entities;
namespace CryptoExchange.Server;

public class CryptoExchangeDbContext : DbContext
{
    public DbSet<OrderBook> OrderBooks => Set<OrderBook>();
    public DbSet<OrderBookItem> OrderBookItems => Set<OrderBookItem>();
    public CryptoExchangeDbContext()
    {

    }
    public CryptoExchangeDbContext(DbContextOptions<CryptoExchangeDbContext> options) : base(options)
    {

    }
}
