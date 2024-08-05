using CryptoExchange.Server;
using Microsoft.EntityFrameworkCore;
using CryptoExchange.Base.Interfaces;
using CryptoExchange.Server.Services;
using NSubstitute;
using CryptoExchange.Server.Entities;
using Microsoft.Extensions.Configuration;
namespace CryptoExchange.Test.Services;

public class BitstampServiceTest
{
    private readonly IDbContextFactory<CryptoExchangeDbContext> _dbContextFactory;
    private readonly IBitstampService _bitstampService;
    public BitstampServiceTest()
    {
        _dbContextFactory = new TestDbContextFactory<CryptoExchangeDbContext>(Guid.NewGuid().ToString());
        var httpClient = Substitute.For<HttpClient>();
        var configuration = Substitute.For<IConfiguration>();
        _bitstampService = new BitstampService(configuration, httpClient, _dbContextFactory);
    }

    [Fact]
    public async Task GetAvaliableTimeStamps_Empty_ShouldReturnEmpty()
    {
        var timestamps = await _bitstampService.GetAvaliableTimeStamps();
        timestamps.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAvaliableTimeStamps_ShouldReturnData()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.OrderBooks.Add(new()
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(633555259920000000, new TimeSpan(3, 2, 0))
        });
        context.OrderBooks.Add(new()
        {
            Id = 999,
            TimeStamp = new DateTimeOffset(633555259920000000, TimeSpan.Zero)
        });
        await context.SaveChangesAsync();

        var expected = new Dictionary<long, DateTimeOffset>
        {
            { 1, new DateTimeOffset(633555259920000000, new TimeSpan(3, 2, 0)) },
            { 999, new DateTimeOffset(633555259920000000, TimeSpan.Zero) }
        };

        var timestamps = await _bitstampService.GetAvaliableTimeStamps();
        timestamps.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetOrderBookById_Empty_ShouldReturnNull()
    {
        var orderBookDto = await _bitstampService.GetOrderBookById(777);
        orderBookDto.Should().BeNull();
    }

    [Fact]
    public async Task GetOrderBookById_ShouldReturnData()
    {
        using var context = await _dbContextFactory.CreateDbContextAsync();
        context.OrderBooks.Add(new()
        {
            Id = 1,
            TimeStamp = new DateTimeOffset(633555259920000000, new TimeSpan(3, 2, 0))
        });
        var orderBook = new OrderBook()
        {
            Id = 999,
            TimeStamp = new DateTimeOffset(633555259920000000, TimeSpan.Zero)
        };
        context.OrderBooks.Add(orderBook);
        var expected = orderBook.ToDto();

        await context.SaveChangesAsync();
        var orderBookDto = await _bitstampService.GetOrderBookById(999);
        orderBookDto.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetOrderBookAsync_Empty_ShouldReturnNull()
    {
        var orderBookDto = await _bitstampService.GetOrderBookAsync("", "");
        orderBookDto.Should().BeNull();
    }

    [Fact]
    public async Task GetOrderBookAsync_ShouldReturnData()
    {
        var orderBookDto = await _bitstampService.GetOrderBookAsync("btc", "eur");
        orderBookDto.Should().NotBeNull();
    }
}
