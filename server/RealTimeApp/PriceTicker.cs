using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

public interface IPriceTicker : IDisposable
{
    Task AddSymbol(string symbol);
    Task RemoveSymbol(string symbol);
    decimal GetPrice(string _);
    public IObservable<Price> PriceStream { get; }
    string[] Symbols();
}

public class PriceTicker : IPriceTicker
{
    private readonly ILogger<PriceTicker> _logger;
    private readonly IScheduler _scheduler;
    private readonly HashSet<string> _symbols = new();
    private readonly Random _random = new();
    private readonly CompositeDisposable _disposables = new();
    //consider a high frequency ticker that we'll have to manage later
    private const double TickingFrequency = 200;


    public PriceTicker(ILogger<PriceTicker> logger, IScheduler scheduler)
    {
        _logger = logger;
        _scheduler = scheduler;
        PriceStream = Start();
    }

    public IObservable<Price> PriceStream { get;  init; }

    public string[] Symbols()
    {
        lock (_symbols)
        {
            return _symbols.ToArray();
        }
    }
    
    
    public async Task AddSymbol(string symbol)
    {
        lock(_symbols)
            _symbols.Add(symbol);
        await Task.CompletedTask;
    }

    private IObservable<Price> Start()
    {
        return Observable.Interval(TimeSpan.FromMilliseconds(TickingFrequency), _scheduler)
            .Select(_ =>
            {
                string[] symbolArray;
                lock (_symbols)
                {
                    if (!_symbols.Any())
                        return null;
                    symbolArray = _symbols.ToArray();
                }

                var index = _random.Next(symbolArray.Length);
                var randomSymbol = symbolArray[index];
                return new Price(randomSymbol, GetRandomPrice());
            })
            .Where(price => price != null)!
            .Cast<Price>()
            // .Do(price => _logger.LogInformation($"Updating price for {price?.Symbol}: {price?.Value}"))
            .Publish()
            .RefCount(); // Make the observable hot
    }

    public async Task RemoveSymbol(string symbol)
    {
        lock(_symbols)
            _symbols.Remove(symbol);
        await Task.CompletedTask;
    }

    private decimal GetRandomPrice()
    {
        return _random.Next(0, 101);
    }

    public decimal GetPrice(string _) => GetRandomPrice();
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
}

public record Price(string Symbol, decimal Value);
