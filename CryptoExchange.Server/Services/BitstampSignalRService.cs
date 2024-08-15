using CryptoExchange.Base;
using CryptoExchange.Base.Interfaces;
using CryptoExchange.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Timers;

namespace CryptoExchange.Server.Services;

public class BitstampSignalRService(IHubContext<OrderBookHub> hubContext, IConfiguration configuration, IBitstampService bitstampService) : IHostedService
{
    private readonly IHubContext<OrderBookHub> _hubContext = hubContext;
    private System.Timers.Timer? Timer;
    private readonly IBitstampService _bitstampService = bitstampService;
    private readonly IConfiguration _configuration = configuration;
    public async Task SendOrderBookToClients()
    {
        var orderbook = await _bitstampService.GetOrderBookAsync("btc", "eur");
        if (orderbook is null)
            return;

        var hub = _hubContext.Clients.All;
        await hub.SendAsync(Constants.SignalR_ReceiveOrderBookMethod, orderbook);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Timer = new()
        {
            Interval = _configuration.GetSection("RequestIntervalInSeconds").Get<double>() * 1000
        };
        Timer.Elapsed += async (object? sender, ElapsedEventArgs e) =>
        {
            await SendOrderBookToClients();
        };
        Timer.Enabled = true;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Timer?.Stop();
        return Task.CompletedTask;
    }
}