import React from 'react';

function Spinner() {
    return (
        <div style={spinnerContainerStyle}>
            <img
                src="/images/spinner.gif"
                alt="Loading..."
                style={spinnerStyle}
            />
        </div>
    );
}

const spinnerContainerStyle = {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    height: '100%', // Adjust the height as needed
    width: '100%',  // Adjust the width as needed
};

const spinnerStyle = {
    width: '50px',  // Adjust the size as needed
    height: '50px', // Adjust the size as needed
};

export default Spinner;
