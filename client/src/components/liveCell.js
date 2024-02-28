import React, { useState, useEffect, useRef } from 'react';
import '../App.css';

export const LiveCell = ({ value, simple = false }) => {
    const currentValueRef = useRef(value); // Initialize with the initial value
    const [change, setChange] = useState(0);
    const [pulsing, setPulsing] = useState(false);

    useEffect(() => {
        if (value === undefined) return;
        
        const diff = value - currentValueRef.current;
        setChange(diff);
        currentValueRef.current = value;
        
        if (diff !== 0) {
            setPulsing(true);
            const timeoutId = setTimeout(() => setPulsing(false), 500); 
            return () => clearTimeout(timeoutId); 
        }
    }, [value]);

    const formattedValue = simple || change === null ? (
        <div>{value}</div>
    ) : (
        <>
            <span>{value}</span>
            <span className='trend'> 
                ({change > 0 ? "+" : "-"}{Math.abs(change.toFixed(2))})
            </span>
        </>
    );

    return (
        <div data-testid="live-cell"
             className={`grid-item ${change < 0 ? 'value-down' : 'value-up'}  ${pulsing ? (change >= 0 ? 'pulsing-up' : 'pulsing-down') : ''}`}
        >
            {formattedValue}
        </div>
    );
};
