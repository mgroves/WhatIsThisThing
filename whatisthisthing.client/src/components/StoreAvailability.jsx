import React from 'react';

const StoreAvailability = ({ store }) => (
    <div style={{ marginTop: '10px' }}>
        <p><strong>{store.storeName}</strong>: {store.quantity > 0 ? store.quantity : <span style={{ color: 'red' }}>Out of Stock</span>}</p>
    </div>
);

export default StoreAvailability;
