# Real-time Price Updates Application - POC

## Description
This application demonstrates real-time price updates for financial symbols (e.g., stocks, currencies) using a React front-end and a .NET back-end. 

## User Features

* Subscribe to symbols and view real-time price updates.
* Unsubscribe from individual symbols.
* Visualize price changes with color and change amount

## Using the Application

1. Enter a symbol in the input field and press Enter or click Add Symbol.
2. The symbol's price updates in real-time with color-coding for price changes.
3. Unsubscribe by clicking the Close button next to the symbol.
4. Test exponential backoff by stopping the server and refreshing the client. You should see reconnection attempts followed by a successful connection or an error report depending on the server state

## Installation and Running

1. Open RealTimeApp.sln in the server folder, in an IDE, build, and run. 
2. Open a terminal, navigate to the client project directory, and run yarn install or npm install. 
3. Run yarn start. 
4. Open localhost:3000 in your browser. 
5. Run yarn test to execute unit tests (server unit tests in RealTimeAppTests project).

## Design Considerations

This is a proof-of-concept application with simplifications. Any string is considered a valid symbol for demonstration purposes.

### Client

* Simple and intuitive UI with keyboard/button input for adding symbols.
* Real-time price updates with animated values, color-coding, and change amount.
* Five components responsible for specific tasks:
  * App: Handles communication with server API (SignalR).
  * useHubConnection: Custom hook for SignalR connection with exponential backoff.
  * LiveCell: Manages price value, formatting, and presentation.
  * SymbolInput: Handles user input for adding symbols.
  * SymbolGrid: Displays live data grid and manages symbol removal.
* Unit tests provided for most components.


### Server

* PriceTicker: Simulates a high-frequency price feed using Rx.
* StockTikerHub: Subscribes to the PriceTicker stream, transforms data, and sends updates to clients efficiently using Rx operators.
* PriceTicker leverages Rx and allows unit testing with TestScheduler.


## Future Architecture Considerations for Performance, Scalability, and Connectivity
While this POC demonstrates the core functionalities, there are several potential improvements for future iterations:

* Performance: Implement real-time data feeds like Reuters or Bloomberg for accurate and low-latency price updates. Consider in-memory data caching (e.g., Redis) for frequently accessed symbols to reduce server load.
* Scalability: Transition to a microservices architecture to enhance modularity and independent scaling of individual services. Consider message queues (e.g., RabbitMQ, Kafka) for decoupling data flow and handling peak loads efficiently.
* Connectivity: Implement robust error handling and retry logic using libraries like .NET Polly in the back-end for handling transient network failures gracefully. Consider server clustering for high availability and redundancy in case of server outages.
