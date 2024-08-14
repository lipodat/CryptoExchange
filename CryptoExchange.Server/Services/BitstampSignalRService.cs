using CryptoExchange.Base;
using CryptoExchange.Base.Interfaces;
using CryptoExchange.Server.Entities.Dto;
using CryptoExchange.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Timers;

namespace CryptoExchange.Server.Services;

public class BitstampSignalRService(IHubContext<OrderBookHub> hubContext, IBitstampService bitstampService) : IHostedService
{
    private readonly IHubContext<OrderBookHub> _hubContext = hubContext;
    private System.Timers.Timer? Timer;
    private readonly IBitstampService _bitstampService = bitstampService;
    public async Task SendOrderBookToClients()
    {
        var orderbook = await _bitstampService.GetOrderBookAsync("btc", "eur");
        if (orderbook is null)
            return;

        var hub = _hubContext.Clients.All;
        await hub.SendAsync(Constants.SignalR_Method, orderbook);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Timer = new()
        {
            Interval = 10000
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