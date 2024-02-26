using Microsoft.AspNetCore.SignalR;

public class StockTickerHubOld : Hub
{
    private readonly HashSet<string> _symbols = new();
    // private PeriodicTimer _updateTimer;// = new(TimeSpan.FromSeconds(1));
    private readonly Random _random = new();
    
    
    public StockTickerHubOld()
    {
        var _ = StartPeriodicUpdates();
    }

    async Task StartPeriodicUpdates()
    {
        while (true)
        {
            await UpdateRandomSymbol();
        }
    }

    async Task UpdateRandomSymbol()
    {
        await Task.Delay(1000);
        string[] symbolArray;
        lock (_symbols)
            symbolArray = _symbols.ToArray();
        
        if (!symbolArray.Any())
            return;

        var index = _random.Next(symbolArray.Length);
        var randomSymbol = symbolArray[index];
        decimal newPrice = GetPrice(randomSymbol);
        Console.WriteLine($"new price {newPrice} for {randomSymbol} {index} {symbolArray.Length}");
        
        await SendUpdate(randomSymbol, newPrice);
        
    }

    public async Task Subscribe(string symbol)
    {
        lock(_symbols)
            _symbols.Add(symbol);
        await Groups.AddToGroupAsync(Context.ConnectionId, symbol);
        // Simulate fetching the current price for the symbol and send it back
        await Clients.Caller.SendAsync("ReceiveUpdate", symbol, GetPrice(symbol));
    }

    public async Task Unsubscribe(string symbol)
    {
        lock(_symbols)
            _symbols.Remove(symbol);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, symbol);
    }

    public Task SendUpdate(string symbol, decimal price)
    {
        return Clients.Group(symbol).SendAsync("ReceiveUpdate", symbol, price);
    }

    private decimal GetPrice(string symbol)
    {
        // Placeholder for getting the initial price of a symbol
        decimal randomNumber = _random.Next(0, 101); // Generates a random number between 0 and 100, inclusive.

        return randomNumber; // Simulate an initial price
    }
    

}
