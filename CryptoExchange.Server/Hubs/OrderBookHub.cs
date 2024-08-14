using CryptoExchange.Base;
using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.SignalR;

namespace CryptoExchange.Server.Hubs
{
    public class OrderBookHub : Hub
    {
        public Task SendMessage(OrderBookDto orderBook)
        {
            return Clients.All.SendAsync(Constants.SignalR_Method, orderBook);
        }
    }
}
