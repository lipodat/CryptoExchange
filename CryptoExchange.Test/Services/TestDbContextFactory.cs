using Microsoft.EntityFrameworkCore;

namespace CryptoExchange.Test.Services;

public class TestDbContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
{
    private readonly DbContextOptions<TContext> _options;
    public TestDbContextFactory(string databaseName = "InMemoryTest")
    {
        _options = new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }

    public TContext CreateDbContext()
    {
        if (Activator.CreateInstance(typeof(TContext), [_options]) is not TContext context)
            throw new InvalidOperationException($"Can't create context of type {typeof(TContext).Name}");
        return context;
    }
}
