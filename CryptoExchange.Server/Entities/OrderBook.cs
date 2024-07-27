using CryptoExchange.Base.Models;

namespace CryptoExchange.Server.Entities
{
    public class OrderBook
    {
        public long Id { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public List<OrderBookItem> Items { get; set; } = [];

        public OrderBook()
        {

        }
        public OrderBook(OrderBookRecord orderBookRecord)
        {
            Id = orderBookRecord.Id;
            TimeStamp = orderBookRecord.TimeStamp;
            Items = [];
            Items.AddRange(orderBookRecord.Bids.Select(x => new OrderBookItem(x, true, this)).ToList());
            Items.AddRange(orderBookRecord.Asks.Select(x => new OrderBookItem(x, false, this)).ToList());
        }
        public OrderBookRecord ToRecord()
        {
            return new()
            {
                Id = Id,
                TimeStamp = TimeStamp,
                Bids = Items.Where(x => x.IsBid).Select(x => x.ToRecord()).ToList(),
                Asks = Items.Where(x => !x.IsBid).Select(x => x.ToRecord()).ToList()
            };
        }
    }
}
