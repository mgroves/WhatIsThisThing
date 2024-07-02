import React, { useState, useEffect } from 'react';
import Item from './Item';

function WhatIsThis({ addToCart, modalInfo }) {
    const [photo, setPhoto] = useState(null);
    const [identifiedItem, setIdentifiedItem] = useState(null);
    const [relatedItems, setRelatedItems] = useState([]);
    const [cart, setCart] = useState([]);
    const [total, setTotal] = useState(0);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        modalInfo("","");
    }, []);

    const handlePhotoUpload = async (event) => {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = async (e) => {
                const originalImage = new Image();
                originalImage.src = e.target.result;

                originalImage.onload = async () => {
                    const canvas = document.createElement('canvas');
                    const context = canvas.getContext('2d');
                    const maxWidth = 640;

                    const ratio = maxWidth / originalImage.width;
                    canvas.width = maxWidth;
                    canvas.height = originalImage.height * ratio;

                    context.drawImage(originalImage, 0, 0, canvas.width, canvas.height);

                    const resizedImageDataUrl = canvas.toDataURL();

                    setPhoto(resizedImageDataUrl);

                    // Get user location
                    navigator.geolocation.getCurrentPosition(async (position) => {
                        const { latitude, longitude } = position.coords;

                        try {
                            setLoading(true);

                            const response = await fetch('/api/identify', {
                                method: 'POST',
                                headers: {
                                    'Content-Type': 'application/json'
                                },
                                body: JSON.stringify({
                                    image: resizedImageDataUrl,
                                    location: { latitude, longitude }
                                })
                            });

                            if (!response.ok) {
                                throw new Error(`HTTP error! Status: ${response.status}`);
                            }

                            const data = await response.json();

                            // Handle the response
                            const { identifiedItem, relatedItems } = data.data;
                            const { modalContent, modalTitle } = data;

                            setIdentifiedItem(identifiedItem);
                            setRelatedItems(relatedItems);
                            modalInfo(modalTitle, modalContent);
                        } catch (error) {
                            console.error('Error identifying the item:', error);
                        } finally {
                            setLoading(false);
                        }
                    });
                };
            };
            reader.readAsDataURL(file);
        }
    };

    return (
        <div className="container mt-5">
            <div className="text-center">
                <h2 className="display-4">What Is This Thing?</h2>
                <p>Upload a photo of the mysterious hardware you want to identify.</p>
            </div>
            <input type="file" accept="image/*;capture=camera" onChange={handlePhotoUpload} />
            {loading && <p>Loading...</p>}
            {photo && !loading && <img src={photo} alt="Uploaded" style={{ width: '64px', maxWidth: '400px', marginTop: '20px' }} />}

            {identifiedItem && (
                <>
                    <h3>Most likely match:</h3>
                    <Item item={{ ...identifiedItem }} addToCart={addToCart} />
                </>
            )}

            {relatedItems.length > 0 && <h3>Related Items:</h3>}
            {relatedItems.length > 0 && (
                <div className="d-flex flex-wrap">
                    {relatedItems.map((item, index) => (
                        <Item key={index} item={item} addToCart={addToCart} />
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
