import React from 'react';
import StoreAvailability from './StoreAvailability';


const Item = ({ item, addToCart }) => (
    <div style={{ marginTop: '20px' }}>
        <h3>{item.title}</h3>
        <img src={item.image} alt={item.name} style={{ width: '100px' }} />
        <p>{item.name}: {item.desc}</p>
        <p>Price: ${item.price}</p>
        <h4>Availability in Nearby Stores</h4>
        {item.stock.map((store, index) => (
            <StoreAvailability key={index} store={store} />
        ))}
        <button onClick={() => addToCart(item)}>Add to Cart</button>
    </div>
);

export default Item;
