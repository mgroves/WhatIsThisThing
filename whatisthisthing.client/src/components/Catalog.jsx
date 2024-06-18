import React, { useState, useEffect, useRef, useCallback } from 'react';
import Item from './Item';

const fetchItems = async (page, location) => {
    try {
        const { latitude, longitude } = location;
        const response = await fetch(`/api/catalog?Page=${page}&Latitude=${latitude}&Longitude=${longitude}`);
        const result = await response.json();
        return result.data;
    } catch (error) {
        console.error("Error fetching data:", error);
        return [];
    }
};

function Catalog({ addToCart }) {
    const [items, setItems] = useState([]);
    const [page, setPage] = useState(0);
    const [hasMore, setHasMore] = useState(true);
    const observer = useRef();

    useEffect(() => {
        const fetchAndSetItems = async () => {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(async (position) => {
                    const location = {
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude
                    };
                    const newItems = await fetchItems(page, location);
                    setItems(prevItems => {
                        const filteredItems = newItems.filter(item => !prevItems.some(existingItem => existingItem.name === item.name));
                        return [...prevItems, ...filteredItems];
                    });
                    if (newItems.length === 0) {
                        setHasMore(false);
                    }
                }, (error) => {
                    console.error("Error getting location:", error);
                    setHasMore(false);
                });
            } else {
                console.error("Geolocation is not supported by this browser.");
                setHasMore(false);
            }
        };

        fetchAndSetItems();
    }, [page]);

    const lastItemRef = useCallback(node => {
        if (observer.current) observer.current.disconnect();
        observer.current = new IntersectionObserver(entries => {
            if (entries[0].isIntersecting && hasMore) {
                setPage(prevPage => prevPage + 1);
            }
        }, {
            rootMargin: '100px', // Adjust this value as needed
            threshold: 0.1 // Adjust this value as needed
        });
        if (node) observer.current.observe(node);
    }, [hasMore]);

    return (
        <div className="container mt-5">
            <div className="text-center">
                <h2 className="display-4">Browse Catalog</h2>
                <p>Check out the entire catalog of items currently stored in Couchbase.</p>
            </div>
            <div className="d-flex flex-wrap">
                {items.map((item, index) => {
                    if (index === items.length - 1) {
                        return (
                            <Item key={index} item={item} addToCart={addToCart} ref={lastItemRef} />
                        );
                    } else {
                        return (
                            <Item key={index} item={item} addToCart={addToCart} />
                        );
                    }
                })}
            </div>
            {!hasMore && <p>All items displayed.</p>}
        </div>
    );
}

export default Catalog;
