import React, { useState, useEffect, useRef, useCallback } from 'react';
import Store from './Store';

const fetchStores = async (page) => {
    try {
        const response = await fetch(`/api/stores?page=${page}`);
        const result = await response.json();
        return result.data;
    } catch (error) {
        console.error("Error fetching data:", error);
        return [];
    }
};

function Stores() {
    const [stores, setStores] = useState([]);
    const [page, setPage] = useState(0);
    const [hasMore, setHasMore] = useState(true);
    const observer = useRef();

    useEffect(() => {
        const fetchAndSetStores = async () => {
            const newStores = await fetchStores(page);
            setStores(prevStores => {
                const filteredStores = newStores.filter(store => !prevStores.some(existingStore => existingStore.name === store.name));
                return [...prevStores, ...filteredStores];
            });
            if (newStores.length === 0) {
                setHasMore(false);
            }
        };

        fetchAndSetStores();
    }, [page]);

    const lastStoreRef = useCallback(node => {
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
                <h2 className="display-4">Browse Stores</h2>
                <p>Check out the entire catalog of stores currently stored in Couchbase.</p>
            </div>
            <div className="d-flex flex-wrap">
                {stores.map((store, index) => {
                    if (index === stores.length - 1) {
                        return (
                            <Store key={index} store={store} ref={lastStoreRef} />
                        );
                    } else {
                        return (
                            <Store key={index} store={store} />
                        );
                    }
                })}
            </div>
            {!hasMore && <p>All stores displayed.</p>}
        </div>
    );
}

export default Stores;
