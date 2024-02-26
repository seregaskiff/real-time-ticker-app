import React, { useState, useEffect, useCallback, useRef } from 'react';
import '../App.css';


export const LiveCell = ({ value, simple=false }) => {
    const currentValueRef = useRef(0);
    const [change, setChange] = useState(0);
    const [pulsing, setPulsing] = useState(false);

    useEffect(() => {
        if (value === undefined)
            return;
        const diff = value - currentValueRef.current;
        setChange(diff);
        currentValueRef.current = value;
        setPulsing(diff !== 0);
        setTimeout(() => setPulsing(false), 500); // Remove class after pulse
    }, [value]);


    const FormattedValue = useCallback(({simple}) => {
        if(simple || change === null)
            return <div>{value}</div>;
        const sign = change > 0 ? "+" : "-";
        return <>
        <span>{value}</span>
        <span className='trend'> ({`${sign}${Math.abs(change.toFixed(2))}`})</span>
        </>
    }, [change, value])

    return (
        <div data-testid="live-cell"
            className={`grid-item ${change < 0 ? 'value-down' : 'value-up'}  ${pulsing ? change >= 0 ? 'pulsing-up' : 'pulsing-down' : ''}`}
        >
            <FormattedValue simple={simple}/>
        </div>
    );
};
