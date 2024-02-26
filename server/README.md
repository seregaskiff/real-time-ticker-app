# Objective

Your task is to develop a simple yet efficient application that allows users to subscribe to real-time price updates for various symbols (e.g., stock, currency). The application will consist of a React-based front-end and a .NET back-end. Users should be able to:

- Enter a symbol to subscribe to real-time price updates.
- View multiple subscribed symbols' prices updating in real-time.
- Unsubscribe from a symbol, removing it from their view.

Template project can be found [here](https://github.com/react-app-tasty-test/react-task).

## Key Requirements

### Front-End

- Use React for the UI development.
- Implement a clean and intuitive interface that allows users to add a new symbol for tracking and view a list of subscribed symbols with their real-time prices.
- Provide an option to unsubscribe from a symbol.

### Back-End

- Use .NET for the server-side logic.
- Ensure the back-end efficiently handles real-time data streaming to multiple clients simultaneously without significant delays.
- Implement mechanisms to manage subscriptions and unsubscriptions to symbols.

### Data Flow

The application should demonstrate efficient real-time data handling, ensuring that the same live data is supplied to different UI clients without noticeable delays.

### Testing

- Include unit tests where necessary to cover key functionality of the application.

## Expectations

- **Efficiency**: The application should be optimized for performance, especially in handling and displaying real-time data.
- **Scalability**: Design the back-end with scalability in mind, capable of handling multiple simultaneous user connections efficiently.
- **Code Quality**: Write clean, readable, and maintainable code. Structure your project clearly and logically.
- **Documentation**: Briefly document your application setup, including how to run it and any prerequisites needed.

## Time Allocation

The task should be completed in a maximum of 2 hours. This time constraint is set to prioritize the implementation of core functionalities and to assess your ability to deliver a proof of concept efficiently.

## Submission Instructions

Upon completion, please submit your code via any publicly available repositories. Include any necessary instructions for running your application and any notes on your design decisions or assumptions.

Good luck, and we look forward to seeing your solution!
