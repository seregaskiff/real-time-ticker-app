using Microsoft.AspNetCore.SignalR;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using RealTimeApp;

public class StockTickerHub : Hub
{
    private readonly IPriceTicker _ticker;
    private readonly ILogger<StockTickerHubOld> _logger;
    //keep all our disposables in one place
    private readonly CompositeDisposable _disposables = new();
    //substantially reduce data flow by managing lower frequency than the ticker
    private const double SampleInterval = 1000;
    
    public StockTickerHub(IPriceTicker ticker, ILogger<StockTickerHubOld> logger)
    {
        _ticker = ticker;
        _logger = logger;
        SubscribeToTicker();
        // SubscribeToTickerMovingAvg();
        //SubscribeToTickerSimple();
    }

    void SubscribeToTicker()
    {
        //here we subscribe to a high frequency ticker, but we want to update prices at much lower rates
        //so we using Rx to dynamically add a timestamp to a price then buffer all prices, group the buffer by symbol 
        //and then take latest prices for each symbol send updates only for those prices 
        _ticker.PriceStream
            .Timestamp()
            .Buffer(TimeSpan.FromMilliseconds(SampleInterval))
            .Subscribe(prices =>
            {
                var grouped = prices.GroupBy(p => p.Value.Symbol);
                var latest = grouped.Select(g => g.MaxBy(p => p.Timestamp)).ToList();
                _logger.LogInformation($"sending {latest.Count} out of {prices.Count}");
                foreach (var price in latest)
                    SendUpdate(price.Value.Symbol, price.Value.Value);
            }).AddToDisposables(_disposables);
        _ticker.AddToDisposables(_disposables);
    }
    
    
    void SubscribeToTickerMovingAvg()
    {
        var windowSize = 10;
        var prices = _ticker.PriceStream
            .Scan(
                new Dictionary<string, (decimal sum, int count)>(), // Initial accumulator
                (acc, price) =>
                {
                    // Update accumulator for the symbol
                    if (!acc.ContainsKey(price.Symbol))
                    {
                        acc[price.Symbol] = (price.Value, 1);
                    }
                    else
                    {
                        var (currentSum, currentCount) = acc[price.Symbol];
                        acc[price.Symbol] =
                            (currentSum + price.Value, ++currentCount);
                    }

                    return acc;
                }
            )
            .SelectMany(prices =>
            {
                return prices.Keys.Select(k => new Price(k, prices[k].sum / prices[k].count));
            })
            .Subscribe(price =>
            {
                _logger.LogInformation($"sending moving average for: {price.Symbol} value: {price.Value}");
                SendUpdate(price.Symbol, price.Value);
            }).AddToDisposables(_disposables);
        _ticker.AddToDisposables(_disposables);
    }
    
    
    void SubscribeToTickerSimple()
    {
        _ticker.PriceStream
            // .Sample(TimeSpan.FromMilliseconds(SampleInterval)) //will make it look nice but we will miss most updates
            .Subscribe(price =>
            {
                _logger.LogInformation($"sending update for: {price.Symbol} value: {price.Value}");
                SendUpdate(price.Symbol, price.Value);
            }).AddToDisposables(_disposables);
        _ticker.AddToDisposables(_disposables);
    }

    public async Task Subscribe(string symbol)
    {
        await _ticker.AddSymbol(symbol);
        await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
        await Clients.Caller.SendAsync("ReceiveUpdate", symbol, _ticker.GetPrice(symbol));
    }

    public async Task Unsubscribe(string symbol)
    {
        await _ticker.RemoveSymbol(symbol);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
    }

    public Task SendUpdate(string symbol, decimal price)
    {
        // _logger.LogInformation($"sending update for symbol: {symbol}, price: {price}");
        return Clients.Group(symbol).SendAsync("ReceiveUpdate", symbol, price);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        //will dispose all items from the collection at once
        _disposables.Dispose();   
    }
}
