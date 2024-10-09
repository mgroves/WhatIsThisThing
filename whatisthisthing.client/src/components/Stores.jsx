import React, { useState, useEffect, useRef, useCallback } from 'react';
import Store from './Store';
import Spinner from './Spinner';

const fetchStores = async (page) => {
    try {
        const response = await fetch(`/api/stores?page=${page}`);
        const result = await response.json();
        return result;
    } catch (error) {
        console.error("Error fetching data:", error);
        return { data: [], modalTitle: null, modalContent: null };
    }
};

function Stores({ modalInfo }) {
    const [stores, setStores] = useState([]);
    const [page, setPage] = useState(0);
    const [hasMore, setHasMore] = useState(true);
    const [loading, setLoading] = useState(false);
    const observer = useRef();

    useEffect(() => {
        modalInfo("", "");

        const fetchAndSetStores = async () => {
            setLoading(true);
            const { data, modalTitle, modalContent } = await fetchStores(page);

            setStores(prevStores => {
                const filteredStores = data.filter(store => !prevStores.some(existingStore => existingStore.name === store.name));
                return [...prevStores, ...filteredStores];
            });
            if (data.length === 0) {
                setHasMore(false);
            } else {
                setHasMore(true); // Reset hasMore if items are fetched
            }

            modalInfo(modalTitle, modalContent);
            setLoading(false);
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
            {loading && <Spinner />}
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