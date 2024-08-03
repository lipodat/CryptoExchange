using CryptoExchange.Base.Models;

namespace CryptoExchange.Server.Entities
{
    public class OrderBook
    {
        public long Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public List<OrderBookItem> Items { get; set; } = [];
        public OrderBookDto ToDto()
        {
            return new()
            {
                Id = Id,
                TimeStamp = TimeStamp,
                Bids = Items.Where(x => x.IsBid).Select(x => x.ToDto()).ToList(),
                Asks = Items.Where(x => !x.IsBid).Select(x => x.ToDto()).ToList()
            };
        }
    }
}
