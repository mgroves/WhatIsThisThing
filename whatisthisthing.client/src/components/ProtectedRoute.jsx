import React from 'react';
import { Navigate } from 'react-router-dom';

function ProtectedRoute({ element: Component, requiredPermission }) {
    var auth = JSON.parse(localStorage.getItem('wittAuth'));

    var hasPermission = !(auth == null); // check to make sure auth exists

    if (!hasPermission) {
        // Redirect to home if no auth loaded
        return <Navigate to="/login" />;
    }

    return <Component />;
}

export default ProtectedRoute;
