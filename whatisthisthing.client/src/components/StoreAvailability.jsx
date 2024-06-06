import React from 'react';

const StoreAvailability = ({ store }) => (
    <p><strong>{store.storeName}</strong>: {store.quantity > 0 ? store.quantity : <span style={{ color: 'red' }}>Out of Stock</span>}</p>
);

export default StoreAvailability;
