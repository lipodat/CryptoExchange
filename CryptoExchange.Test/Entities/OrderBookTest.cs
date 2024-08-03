using CryptoExchange.Server.Entities;
using CryptoExchange.Base.Models;

namespace CryptoExchange.Test.Entities;

public class OrderBookTest
{
    [Theory]
    [ClassData(typeof(OrderBooksGenerator))]
    public void OrderBook_ConvertToDto_ReturnsOrderBookDto(OrderBook input, OrderBookDto expected)
    {
        input.ToDto().Should().BeEquivalentTo(expected);
    }
    public class OrderBooksGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _list =
            [
                [
                    new OrderBook()
                    {
                        TimeStamp = new DateTimeOffset(633555259920000000, new TimeSpan(3, 2, 0)),
                        Items =
                        [
                            new() { Price = 1d, Amount = 555d },
                            new() { Price = 2d, Amount = 333d },
                            new() { Price = 122d, Amount = 55533d, IsBid = true },
                            new() { Price = 4.2d, Amount = 0.333d, IsBid = true }
                        ]
                    },
                    new OrderBookDto()
                    {
                        TimeStamp = new DateTimeOffset(633555259920000000, new TimeSpan(3, 2, 0)),
                        Asks =
                        [
                            new() { Price = 1d, Amount = 555d },
                            new() { Price = 2d, Amount = 333d }
                        ],
                        Bids =
                        [
                            new() { Price = 122d, Amount = 55533d},
                            new() { Price = 4.2d, Amount = 0.333d}
                        ]
                    },
                ],
                [
                    new OrderBook()
                    {
                        TimeStamp = new DateTimeOffset(633452259920000000, TimeSpan.Zero),
                        Items =
                        [
                            new() { Price = 111.9999d, Amount = 7.4d },
                            new() { Price = 27d, Amount = 11d },
                            new() { Price = 999999999d, Amount = 113d, IsBid = true },
                            new() { Price = 42.2d, Amount = 833d, IsBid = true }
                        ]
                    },
                    new OrderBookDto()
                    {
                        TimeStamp = new DateTimeOffset(633452259920000000, TimeSpan.Zero),
                        Asks =
                        [
                            new() { Price = 111.9999d, Amount = 7.4d },
                            new() { Price = 27d, Amount = 11d }
                        ],
                        Bids =
                        [
                            new() { Price = 999999999d, Amount = 113d },
                            new() { Price = 42.2d, Amount = 833d }
                        ]
                    }
                ],
                [
                    new OrderBook()
                    {
                        TimeStamp = new DateTimeOffset(777452259920000000, new TimeSpan(1, 0, 0)),
                        Items = [new() { Price = 8525.77275637091d, Amount = 3235.99930332589d },
                            new() { Price = 5069.273035170258d, Amount = 2509.5901044395296d },
                            new() { Price = 4115.6397185294345d, Amount = 4314.004931506571d, IsBid = true },
                            new() { Price = 930.360582214067d, Amount = 8340.336525460027d, IsBid = true }
                        ]
                    },
                    new OrderBookDto()
                    {
                        TimeStamp = new DateTimeOffset(777452259920000000, new TimeSpan(1, 0, 0)),
                        Asks =
                        [
                            new() { Price = 8525.77275637091d, Amount = 3235.99930332589d },
                            new() { Price = 5069.273035170258d, Amount = 2509.5901044395296d }
                            ],
                        Bids =
                        [
                            new() { Price = 4115.6397185294345d, Amount = 4314.004931506571d },
                            new() { Price = 930.360582214067d, Amount = 8340.336525460027d }
                        ]
                    }
                ]
            ];

        public IEnumerator<object[]> GetEnumerator()
        { return _list.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}
