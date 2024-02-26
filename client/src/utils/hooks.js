import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useState, useEffect, useRef } from 'react';

//provides an exponential backoff policy
//the default one will retry in 8 times in 1, 2, 4, 8, 16, 32, 64, 128 seconds intervals
//to implement backoff in equal intervals define base == 1
const reconnectPolicy = (base = 2, retryCount = 5, interval=1000) => {
    const numbers = [...Array(retryCount + 1).keys()];
    return numbers.map(n => Math.round(Math.pow(base, n) * interval));
}


export const useHubConnection = (url, retryCount = 3, base = 2, interval=1000) => {
    const [initialConnection, setInitialConnection] = useState(null);
    const [connection, setConnection] = useState(null);
    const [error, setError] = useState(null);

    const connectionRef = useRef(null);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl(url)
            .configureLogging(LogLevel.Information) // Optional logging configuration
            .withAutomaticReconnect(reconnectPolicy())
            .build();
        setInitialConnection(newConnection);
        return () => {
            newConnection?.stop();
        };
    }, [url])

    useEffect(() => {
        if (!initialConnection)
            return;
        let retries = 0;
        let connectionCreated = false;
        const createConnection = async () => {
            try {
                const newConnection = initialConnection;
                connectionRef.current = newConnection;
                await newConnection.start();
                setConnection(newConnection);
                setError(null);
                connectionCreated = true;
            } catch (err) {
                //using same retrying algorithm that is in reconnectPolicy
                if (retries < retryCount) {
                    retries++;
                    const delay = interval * Math.pow(base, retries); // Exponential backoff
                    const warn = `Failed to connect, retrying ${retryCount} times. current retry count:${retries} delay: ${delay}`;
                    console.warn(warn);
                    setTimeout(createConnection, delay);
                } else {
                    const err = `Failed to connect after ${retryCount} retries`;
                    console.error(err);
                    setError(new Error(err));
                }
            }
        };

        if (!connectionCreated) {
            createConnection();
        }


    }, [url, initialConnection, retryCount, base, interval]);

    return { connection, error };
};