import React from 'react';
import { SymbolInput } from './symbolInput';
import { LiveCell } from './liveCell';


export const SymbolGrid = ({ addSymbol, removeSymbol, symbols }) => {
    return <div className="App">
        <SymbolInput addSymbol={addSymbol} />
        <div className="grid">
            <div className="grid-header">
                <div className="grid-item">Symbol</div>
                <div className="grid-item">Price</div>
                <div className="grid-item">Actions</div>
            </div>
            {symbols.map((symbol, index) => (
                <div key={index} className="grid-row">
                    <div className="grid-item">{symbol.name}</div>
                    <LiveCell value={symbol.value?.toFixed(2)} />
                    <div className="grid-item">
                        <button onClick={() => removeSymbol(symbol.name)}>Close</button>
                    </div>
                </div>
            ))}
        </div>
    </div>
}

