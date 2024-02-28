import React, { useEffect, useRef } from 'react';



export const SymbolInput = ({ addSymbol }) => {
    const ref = useRef(null);

    useEffect(() => {
        ref.current?.focus();
    }, []);

    const onAddSymbol = (e) => {
        e.preventDefault();
        const symbol = ref.current.value.trim();
        if (symbol) {
            addSymbol(symbol);
            ref.current.value = "";
        }
        ref.current.focus();
    };

    const onKeyDown = (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            onAddSymbol(e);
        }
    };

    return (
        <>
            <input
                ref={ref}
                type="text"
                placeholder="Enter Symbol"
                onKeyDown={onKeyDown} />
            <button onClick={onAddSymbol}>Add Symbol</button>
        </>
    );
};
