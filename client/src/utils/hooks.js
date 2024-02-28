import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useState, useEffect, useRef } from 'react';

// Provides an exponential backoff policy for reconnection attempts
const reconnectPolicy = (base = 2, retryCount = 5, interval = 1000) => {
    return [...Array(retryCount).keys()].map(n => Math.pow(base, n) * interval);
};

export const useHubConnection = (url, retryCount = 3, base = 2, interval = 1000) => {
    const [connection, setConnection] = useState(null);
    const [error, setError] = useState(null);
    const connectionRef = useRef(null);

    useEffect(() => {
        let isActive = true; // flag to prevent state update after unmount
        const delays = reconnectPolicy(base, retryCount, interval); // falculate once outside attemptConnection

        const newConnection = new HubConnectionBuilder()
            .withUrl(url)
            .configureLogging(LogLevel.Information) // fptional: Customize logging level
            .withAutomaticReconnect(reconnectPolicy(base, retryCount, interval)) // apply custom reconnect policy
            .build();

        const attemptConnection = async (attemptsLeft = retryCount) => {
            try {
                await newConnection.start();
                if (isActive) {
                    setConnection(newConnection);
                    setError(null);
                }
            } catch (err) {
                if (attemptsLeft > 0) {
                    const delay = delays[retryCount - attemptsLeft];
                    const warn = `Failed to connect, retrying ${retryCount} times. attemps left:${attemptsLeft} delay: ${delay}`;
                    console.warn(warn);
                    setTimeout(() => attemptConnection(attemptsLeft - 1), delay);
                } else {
                    if (isActive) {
                        setError(new Error(`Failed to connect after ${retryCount} retries`));
                    }
                }
            }
        };

        attemptConnection();

        connectionRef.current = newConnection;

        return () => {
            isActive = false;
            newConnection.stop();
        };
    }, [url, retryCount, base, interval]);

    return { connection, error };
};
