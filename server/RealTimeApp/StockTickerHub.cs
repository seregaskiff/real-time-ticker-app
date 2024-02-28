using Microsoft.AspNetCore.SignalR;
using System.Reactive.Disposables;
using RealTimeApp;
    
public class StockTickerHub : Hub
{
    private readonly IPriceTicker _ticker;

    private readonly ILogger<StockTickerHub> _logger;

    //keep all our disposables in one place
    private readonly CompositeDisposable _disposables = new();

    //substantially reduce data flow by managing lower frequency than the ticker
    private const double SampleInterval = 2000;
    private const int MovingAverageWindowSize = 5;
    
    public StockTickerHub(IPriceTicker ticker, ILogger<StockTickerHub> logger)
    {
        _ticker = ticker.AddToDisposables(_disposables);
        _logger = logger;
        // SubscribeToPriceThrottled();
        SubscribeToAveragePriceThrottled();
    }

    void SubscribeToPriceThrottled()
    {
        PriceStreamAdapter.PriceThrottled(_ticker.PriceStream, SampleInterval)
            .Subscribe(price =>
            {
                _logger.LogInformation($"Sending update for symbol: {price.Symbol}, Latest Price: {price.Value}");
                SendUpdate(price.Symbol, price.Value);
            }, onError: error => _logger.LogError($"Error in subscription: {error.Message}"))
            .AddToDisposables(_disposables); // Ensure proper disposal.
    }    
    
    void SubscribeToAveragePriceThrottled()
    {
        PriceStreamAdapter.PriceThrottled(PriceStreamAdapter.PriceWithMovingAverage(_ticker.PriceStream, MovingAverageWindowSize), SampleInterval)
            .Subscribe(price =>
            {
                _logger.LogInformation($"Sending update for symbol: {price.Symbol}, Latest Average Price: {price.Value}");
                SendUpdate(price.Symbol, price.Value);
            }, onError: error => _logger.LogError($"Error in subscription: {error.Message}"))
            .AddToDisposables(_disposables); // Ensure proper disposal.
    }

    
    public async Task Subscribe(string symbol)
    {
        _logger.LogInformation($"adding subscription {symbol}");
        await _ticker.AddSymbol(symbol);
        await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
        await Clients.Caller.SendAsync("ReceiveUpdate", symbol, _ticker.GetPrice(symbol));
    }

    public async Task Unsubscribe(string symbol)
    {
        _logger.LogInformation($"removing subscription {symbol}");
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