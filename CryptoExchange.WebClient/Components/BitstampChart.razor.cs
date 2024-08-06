using BlazorBootstrap;
using CryptoExchange.Base.Models;
using Microsoft.AspNetCore.Components;

namespace CryptoExchange.WebClient.Components;

public partial class BitstampChart
{
    [Parameter]
    public OrderBookDto OrderBook { get; set; } = new();
    private BarChart barChart = default!;
    private bool _firstRun = true;
    protected override async Task OnParametersSetAsync()
    {
        await RenderChartAsync();
    }
    private async Task RenderChartAsync()
    {
        if (barChart is null)
            return;
        var topTenAsks = OrderBook.Asks.OrderBy(x => x.Price).Take(10);
        var firstTenBids = OrderBook.Bids.OrderByDescending(x => x.Price).Take(10).OrderBy(x => x.Price);
        var data = new ChartData
        {
            Labels = [.. firstTenBids.Select(x => x.Price.ToString()), .. topTenAsks.Select(x => x.Price.ToString())],
            Datasets =
                [
                    new BarChartDataset()
                    {
                        Label = "Bids",
                        Data = [ ..firstTenBids.Select(x => x.Amount) ],
                        BackgroundColor = ["rgb(34, 139, 34)"],
                        CategoryPercentage = 0.8,
                        BarPercentage = 1
                    },
                    new BarChartDataset()
                    {
                        Label = "Asks",
                        Data = [.. topTenAsks.Select(_ => 0), .. topTenAsks.Select(x => x.Amount) ],
                        BackgroundColor = ["rgb(178, 34, 34)"],
                        CategoryPercentage = 0.8,
                        BarPercentage = 1
                    }
                ],
        };

        var options = new BarChartOptions();

        options.Interaction.Mode = InteractionMode.Index;

        options.Responsive = true;

        options.Scales.X!.Title = new ChartAxesTitle { Text = "Price", Display = true };
        options.Scales.Y!.Title = new ChartAxesTitle { Text = "Amount", Display = true };
        options.Scales.X!.Stacked = true;


        if (!_firstRun)
            await barChart.UpdateAsync(data, options);
        else
        {
            await barChart.InitializeAsync(data, options);
            _firstRun = false;
        }
    }
}
