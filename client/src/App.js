import React, { useState, useEffect } from 'react';
import { SymbolGrid } from './components/symbolGrid';
import { useHubConnection } from './utils/hooks';
import './App.css';

function App() {
  const [symbols, setSymbols] = useState([]);
  const { connection, error } = useHubConnection('http://localhost:5063/stockTickerHub');

  useEffect(() => {
    if (error || !connection) return;

    const receiveUpdate = (updatedSymbol, value) => {
      setSymbols(currentSymbols =>
        currentSymbols.map(symbol =>
          symbol.name === updatedSymbol ? { ...symbol, value } : symbol
        )
      );
    };

    connection.on("ReceiveUpdate", receiveUpdate);

    return () => {
      connection.off("ReceiveUpdate", receiveUpdate); // Cleanup to prevent memory leaks
    };
  }, [connection, error]); 
  const addSymbol = symbol => {
    if (symbol && connection) {
      const newSymbol = { name: symbol, value: 0 };
      setSymbols(prevSymbols => [...prevSymbols, newSymbol]);
      connection.invoke("Subscribe", symbol).catch(err => console.error(err));
    }
  };

  const removeSymbol = symbolToRemove => {
    setSymbols(symbols => symbols.filter(sym => sym.name !== symbolToRemove));
    if (connection) {
      connection.invoke("Unsubscribe", symbolToRemove).catch(err => console.error(err));
    }
  };

  if (error) {
    return <div className='error-message'>Error: {error.message}</div>;
  }

  if (!connection) {
    return <div className='connecting-message'>Connecting...</div>;
  }

  return (
    <div className="App">
      <SymbolGrid addSymbol={addSymbol} removeSymbol={removeSymbol} symbols={symbols} />
    </div>
  );
}

export default App;
