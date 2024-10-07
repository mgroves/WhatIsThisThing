import React from 'react';
import { Navigate } from 'react-router-dom';

function ProtectedRoute({ element: Component }) {
    var auth = JSON.parse(localStorage.getItem('wittAuth'));

    if (!auth) {
        // Redirect to login if no auth loaded
        return <Navigate to="/login" />;
    }

    // Extract the expiration from the top level of auth
    const expiration = auth.expiration;

    // Check if the current time is past the expiration time
    const isTokenExpired = Date.now() > expiration;

    if (isTokenExpired) {
        // Clear out the local storage if the token is expired
        localStorage.removeItem('wittAuth');
        return <Navigate to="/login" />;
    }

    // If token is valid and not expired, render the component
    return <Component />;
}

export default ProtectedRoute;
