import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.jsx'
import './index.css'
import { GoogleOAuthProvider } from '@react-oauth/google';

ReactDOM.createRoot(document.getElementById('root')).render(
    {/* If you plan to use the admin/auth area, you'll need to put your own Google client ID here (as well as in the backend) */}
    <GoogleOAuthProvider clientId="818950712399-29tikmiav0gkhpim4au86gd02oui5he6.apps.googleusercontent.com">
        <React.StrictMode>
            <App />
        </React.StrictMode>
    </GoogleOAuthProvider>,
)
