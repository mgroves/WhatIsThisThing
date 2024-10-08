﻿import React, { useState, useEffect, useRef, useCallback } from 'react';
import Item from './Item';
import Spinner from './Spinner';

const fetchItems = async (page, location, priceRange, rating) => {
    try {
        const { latitude, longitude } = location;
        let queryString = `/api/catalog?Page=${page}&Latitude=${latitude}&Longitude=${longitude}`;
        if (priceRange.min !== undefined) {
            queryString += `&minPrice=${priceRange.min}`;
        }
        if (priceRange.max !== undefined) {
            queryString += `&maxPrice=${priceRange.max}`;
        }
        if (rating !== undefined) {
            queryString += `&minRating=${rating}`;
        }
        const response = await fetch(queryString);
        const result = await response.json();
        return result;
    } catch (error) {
        console.error("Error fetching data:", error);
        return { data: [], modalTitle: null, modalContent: null };
    }
};

function Catalog({ addToCart, modalInfo }) {
    const [items, setItems] = useState([]);
    const [page, setPage] = useState(0);
    const [hasMore, setHasMore] = useState(true);
    const [priceRange, setPriceRange] = useState({});
    const [rating, setRating] = useState();
    const [loading, setLoading] = useState(false); // Add loading state
    const observer = useRef();

    useEffect(() => {
        modalInfo("", "");
    }, []);

    useEffect(() => {
        const fetchAndSetItems = async () => {
            setLoading(true); // Set loading to true when fetching starts
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(async (position) => {
                    const location = {
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude
                    };
                    const { data, modalContent, modalTitle } = await fetchItems(page, location, priceRange, rating);

                    setItems(prevItems => {
                        if (page === 0) {
                            return data;
                        } else {
                            const filteredItems = data.filter(item => !prevItems.some(existingItem => existingItem.name === item.name));
                            return [...prevItems, ...filteredItems];
                        }
                    });

                    if (data.length === 0) {
                        setHasMore(false);
                    } else {
                        modalInfo(modalTitle, modalContent);
                        setHasMore(true); // Reset hasMore if items are fetched
                    }
                    setLoading(false); // Set loading to false after fetching completes
                }, (error) => {
                    console.error("Error getting location:", error);
                    setHasMore(false);
                    setLoading(false); // Set loading to false on error
                });
            } else {
                console.error("Geolocation is not supported by this browser.");
                setHasMore(false);
                setLoading(false); // Set loading to false if geolocation is not supported
            }
        };

        fetchAndSetItems();
    }, [page, priceRange, rating]);

    const lastItemRef = useCallback(node => {
        if (observer.current) observer.current.disconnect();
        observer.current = new IntersectionObserver(entries => {
            if (entries[0].isIntersecting && hasMore) {
                setPage(prevPage => prevPage + 1);
            }
        }, {
            rootMargin: '100px',
            threshold: 0.1
        });
        if (node) observer.current.observe(node);
    }, [hasMore]);

    const handlePriceChange = (min, max) => {
        setPage(0);
        setPriceRange({ min, max });
        setItems([]); // Reset items on filter change
    };

    const handleRatingChange = (newRating) => {
        setPage(0);
        setRating(newRating || undefined); // Set rating to undefined if newRating is empty
        setItems([]); // Reset items on filter change
    };

    const getStarEmoji = (rating) => {
        return '⭐'.repeat(rating);
    };

    return (
        <div className="container mt-5">
            <div className="text-center">
                <h2 className="display-4">Browse Catalog</h2>
                <p>Check out the entire catalog of items currently stored in Couchbase.</p>
            </div>
            <div className="d-flex justify-content-center align-items-center mb-4">
                <div className="me-3">
                    <label className="form-label">Price Range:</label>
                    <div className="d-flex">
                        <input
                            type="number"
                            className="form-control me-2"
                            value={priceRange.min || ''}
                            onChange={(e) => handlePriceChange(parseInt(e.target.value), priceRange.max)}
                            placeholder="Minimum Price"
                        />
                        <input
                            type="number"
                            className="form-control"
                            value={priceRange.max || ''}
                            onChange={(e) => handlePriceChange(priceRange.min, parseInt(e.target.value))}
                            placeholder="Maximum Price"
                        />
                    </div>
                </div>
                <div>
                    <label className="form-label">Min Rating:</label>
                    <select
                        className="form-select"
                        value={rating || ''}
                        onChange={(e) => handleRatingChange(parseInt(e.target.value) || '')}
                    >
                        <option value="">Select Rating</option>
                        {[5, 4, 3, 2, 1].map(star => (
                            <option key={star} value={star}>{getStarEmoji(star)}</option>
                        ))}
                    </select>
                </div>
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
            {loading && <div className="text-center my-4"><Spinner /></div>}
            {!hasMore && !loading && <p>All items displayed.</p>}
        </div>
    );
}

export default Catalog;
