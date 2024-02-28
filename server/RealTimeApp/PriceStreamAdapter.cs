using System.Reactive.Linq;

namespace RealTimeApp;

public static class PriceStreamAdapter
{
    public static IObservable<Price> PriceThrottled(IObservable<Price> priceStream, double sampleInterval)
    {
        if (priceStream == null)
            throw new InvalidOperationException("PriceStream is not initialized.");
        return priceStream
            .GroupBy(price => price.Symbol)
            .SelectMany(group => group.Throttle(TimeSpan.FromMilliseconds(sampleInterval)));
    }
    
    public static IObservable<Price> PriceWithMovingAverage(IObservable<Price> priceStream, int windowSize)
    {
        if (priceStream == null)
            throw new InvalidOperationException("PriceStream is not initialized.");
        return priceStream
            .GroupBy(price => price.Symbol)
            // for each group, we apply a windowed buffer to maintain the last 'windowSize' prices.
            .SelectMany(group => group
                // buffer 'windowSize' prices, sliding one price at a time.
                .Buffer(windowSize, 1)
                // ensure we only proceed when we have a full window of prices.
                .Where(buffer => buffer.Count == windowSize)
                .Select(buffer => new
                {
                    Symbol = group.Key,
                    AveragePrice = buffer.Average(p => p.Value),
                    LatestPrice = buffer.Last().Value
                })
                // .Do(p => _logger.LogInformation($"Symbol: {p.Symbol} Latest: {p.LatestPrice} Avg: {p.AveragePrice}"))
                //convert back to our price object (we can enhance it later if needed)
                .Select(p => new Price(p.Symbol, p.AveragePrice)));
    }  
}