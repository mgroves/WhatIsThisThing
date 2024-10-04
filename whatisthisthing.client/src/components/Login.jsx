import React, { useState } from 'react';
import { GoogleLogin } from '@react-oauth/google';
import { verifyToken } from './authBackend';
import Spinner from './Spinner';

function Login() {
    const [errorMessage, setErrorMessage] = useState(null);
    const [loading, setLoading] = useState(false);

    function localStoreWithTtlOneHour(key, jwtToken) {
        const expirationTime = new Date().getTime() + (1 * 60 * 60 * 1000); // Current time + 1 hour
        const tokenData = {
            token: jwtToken,
            expiration: expirationTime
        };
        localStorage.setItem(key, JSON.stringify(tokenData));
    }

    const handleTokenValidation = async (credential) => {
        setLoading(true);
        setErrorMessage(null);

        try {
            const result = await verifyToken(credential);
            localStoreWithTtlOneHour('wittAuth', result);
            window.location.href = '/admin';
        } catch (error) {
            setErrorMessage(error.message);
            console.error('Token validation failed:', error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <h2>Login</h2>
            <p>Please login with your Google account.</p>
            {errorMessage && <p style={{ color: 'red' }}>{errorMessage}</p>}
            {loading ? (
                <Spinner />
            ) : (
                <GoogleLogin
                    onSuccess={resp => handleTokenValidation(resp.credential)}
                    onError={() => {
                        setErrorMessage('Login failed');
                    }}
                />
            )}
        </>
    );
}

export default Login;
