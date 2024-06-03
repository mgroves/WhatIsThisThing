import React, { useState } from 'react';
import Item from './Item';

function WhatIsThis() {
    const [photo, setPhoto] = useState(null);
    const [identifiedItem, setIdentifiedItem] = useState(null);
    const [relatedItems, setRelatedItems] = useState([]);
    const [cart, setCart] = useState([]);
    const [total, setTotal] = useState(0);
    const [loading, setLoading] = useState(false);

    const handlePhotoUpload = async (event) => {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = async (e) => {
                setPhoto(e.target.result);

                try {
                    setLoading(true);

                    const response = await fetch('https://localhost:7145/identify', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ image: e.target.result })
                    });

                    if (!response.ok) {
                        throw new Error(`HTTP error! Status: ${response.status}`);
                    }

                    const data = await response.json();

                    // Handle the response
                    const { identifiedItem, relatedItems } = data.data;
                    setIdentifiedItem(identifiedItem);
                    setRelatedItems(relatedItems);
                } catch (error) {
                    console.error('Error identifying the item:', error);
                } finally {
                    setLoading(false);
                }
            };
            reader.readAsDataURL(file);
        }
    };


    const addToCart = (item) => {
        setCart([...cart, item]);
        setTotal(total + item.price);
    };

    return (
        <div style={{ padding: '20px' }}>
            <h2>What is this?</h2>
            <p>Upload a photo of the mysterious hardware you want to identify.</p>
            <input type="file" accept="image/*" onChange={handlePhotoUpload} />
            {loading && <p>Loading...</p>}
            {photo && !loading && <img src={photo} alt="Uploaded" style={{ width: '100%', maxWidth: '400px', marginTop: '20px' }} />}

            {identifiedItem && <Item item={{ ...identifiedItem, title: 'Identified Item' }} addToCart={addToCart} />}
            {relatedItems.length > 0 && (
                <div style={{ marginTop: '20px' }}>
                    <h3>Related Items</h3>
                    {relatedItems.map((item, index) => (
                        <div key={index} style={{ border: '1px solid #ccc', padding: '10px', marginTop: '10px' }}>
                            <Item item={{ ...item, title: 'Related Item' }} addToCart={addToCart} />
                        </div>
                    ))}
                </div>
            )}

            {cart.length > 0 && (
                <div style={{ marginTop: '20px' }}>
                    <h3>Cart</h3>
                    {cart.map((item, index) => (
                        <div key={index} style={{ border: '1px solid #ccc', padding: '10px', marginTop: '10px' }}>
                            <p>{item.name}</p>
                            <p>Price: ${item.price}</p>
                        </div>
                    ))}
                    <h4>Total: ${total.toFixed(2)}</h4>
                </div>
            )}
        </div>
    );
}

export default WhatIsThis;
