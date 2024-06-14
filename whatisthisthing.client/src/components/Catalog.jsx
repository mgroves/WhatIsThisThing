import React, { useState, useEffect, useRef, useCallback } from 'react';
import Item from './Item';

function Catalog({ addToCart }) {
    const [items, setItems] = useState([]);
    const [page, setPage] = useState(0);
    const [hasMore, setHasMore] = useState(true);
    const observer = useRef();

    useEffect(() => {
        const fetchItems = async () => {
            try {
                const response = await fetch(`/api/catalog/${page}`);
                const result = await response.json();
                setItems(prevItems => {
                    const newItems = result.data.filter(item => !prevItems.some(existingItem => existingItem.name === item.name));
                    return [...prevItems, ...newItems];
                });
                if (result.data.length === 0) {
                    setHasMore(false);
                }
            } catch (error) {
                console.error("Error fetching data:", error);
                setHasMore(false);
            }
        };

        fetchItems();
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
        <div className="container">
            <h2>Browse Catalog</h2>
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
                {!hasMore && <p>No more items to show</p>}
            </div>
        </div>
    );
}

export default Catalog;
