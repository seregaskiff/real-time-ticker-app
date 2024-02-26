using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using Moq;


namespace RealTimeAppTests
{
    public class PriceTickerTests
    {
        [SetUp]
        public void Setup()
        {
        }
        
        
        // Ensure logger and scheduler are injected and PriceStream is initialized
        [Test]
        public void ShouldInitialize()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PriceTicker>>();
            var mockScheduler = new TestScheduler();

            // Act
            var priceTicker = new PriceTicker(mockLogger.Object, mockScheduler);

            // Assert
            Assert.NotNull(priceTicker.PriceStream);
        }

        [Test]
        public async Task ShouldAddsSymbolToSet()
        {
            // Arrange
            var priceTicker = new PriceTicker(Mock.Of<ILogger<PriceTicker>>(), Mock.Of<IScheduler>());
            var symbol = "Symbol";

            // Act
            await priceTicker.AddSymbol(symbol);

            // Assert
            Assert.Contains(symbol, priceTicker.Symbols());
        }

        [Test]
        public async Task ShouldHandleDuplicateSymbols()
        {
            // Arrange
            var priceTicker = new PriceTicker(Mock.Of<ILogger<PriceTicker>>(), Mock.Of<IScheduler>());
            var symbol = "Symbol";

            // Act
            await priceTicker.AddSymbol(symbol);
            await priceTicker.AddSymbol(symbol);

            // Assert
            Assert.That(priceTicker.Symbols().Length, Is.EqualTo(1));
        }

        [Test]
        public async Task ShouldRemoveSymbolt()
        {
            // Arrange
            var priceTicker = new PriceTicker(Mock.Of<ILogger<PriceTicker>>(), Mock.Of<IScheduler>());
            var symbol = "Symbol";
            await priceTicker.AddSymbol(symbol); // Add directly for testing

            // Act
            await priceTicker.RemoveSymbol(symbol);

            // Assert
            Assert.That(priceTicker.Symbols(),  Does.Not.Contain(symbol));
        }

        [Test]
        public async Task ShouldHandleNonExistentSymbolWhenRemoving()
        {
            // Arrange
            var priceTicker = new PriceTicker(Mock.Of<ILogger<PriceTicker>>(), Mock.Of<IScheduler>());
            var symbol = "NOT_THERE";

            // Act
            await priceTicker.RemoveSymbol(symbol);

            // Assert
            Assert.That(priceTicker.Symbols().Length, Is.EqualTo(0)); // No symbols added
        }
        
        [Test]
        public async Task ShouldTickPricesForAddedSymbols()
        {
            // Arrange
            var testScheduler = new TestScheduler();
            var priceTicker = new PriceTicker(Mock.Of<ILogger<PriceTicker>>(), testScheduler);
            var symbol = "Symbol";
            List<Price> receivedPrices = new List<Price>();

            // Act
            await priceTicker.AddSymbol(symbol);
            priceTicker.PriceStream.Take(2).ObserveOn(testScheduler).Subscribe(price => receivedPrices.Add(price));
            testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(200 * 3).Ticks); // Advance time for three emissions

            // Assert
            Assert.That(receivedPrices.Count, Is.EqualTo(2));
            Assert.That(receivedPrices[0].Symbol, Is.EqualTo(symbol));
            Assert.That(receivedPrices[1].Symbol, Is.EqualTo(symbol));
        }


    }
}