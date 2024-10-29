import React, { useState, useEffect } from 'react';
import Spinner from '../Spinner';

const Stock = ({ currentPage, setCurrentPage }) => {
    const [stocks, setStocks] = useState([]);
    const [newStock, setNewStock] = useState({ itemId: '', storeId: '', numInStock: '' });
    const [editingStockIndex, setEditingStockIndex] = useState(null);
    const [items, setItems] = useState([]);
    const [stores, setStores] = useState([]);
    const [loading, setLoading] = useState(false);
    const [totalPages, setTotalPages] = useState(1);

    useEffect(() => {
        fetchStocks(currentPage);
        fetchDropdownData();
    }, [currentPage]);

    const getJwtToken = () => {
        const authData = JSON.parse(localStorage.getItem('wittAuth'));
        return authData ? authData.token.jwtToken : null;
    };

    const fetchStocks = async (page = 0) => {
        setLoading(true);
        const token = getJwtToken();
        try {
            const response = await fetch(`/api/admin/stock?page=${page}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            if (response.ok) {
                const data = await response.json();
                setStocks(data.stock);
                setTotalPages(data.totalPages);
            } else {
                console.error("Failed to fetch stocks.");
            }
        } catch (error) {
            console.error("Error fetching stocks:", error);
        } finally {
            setLoading(false);
        }
    };

    const fetchDropdownData = async () => {
        const token = getJwtToken();
        const cacheExpiryTime = 300000; // 5 minutes in milliseconds

        const fetchFromApi = async (url, cacheKey) => {
            const cachedData = JSON.parse(sessionStorage.getItem(cacheKey));
            const currentTime = Date.now();

            // Check if cached data exists and is still valid (within the 5-minute timeframe)
            if (cachedData && (currentTime - cachedData.timestamp < cacheExpiryTime)) {
                return cachedData.items;
            }

            try {
                const response = await fetch(url, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });
                if (response.ok) {
                    const data = await response.json();
                    const cacheObject = {
                        items: data,
                        timestamp: currentTime
                    };
                    sessionStorage.setItem(cacheKey, JSON.stringify(cacheObject));
                    return data;
                }
            } catch (error) {
                console.error(`Error fetching ${cacheKey}:`, error);
            }

            return [];
        };

        const fetchedItems = await fetchFromApi('/api/admin/stock/allItems', 'itemsCache');
        const fetchedStores = await fetchFromApi('/api/admin/stock/allStores', 'storesCache');
        setItems(fetchedItems);
        setStores(fetchedStores);
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        setLoading(true);
        await submitNewStock(newStock);
    };

    const submitNewStock = async (stock) => {
        const token = getJwtToken();
        try {
            await fetch('/api/admin/stock', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    itemId: stock.itemId,
                    storeId: stock.storeId,
                    numInStock: parseInt(stock.numInStock, 10)
                })
            });
            setNewStock({ itemId: '', storeId: '', numInStock: '' });
            fetchStocks(currentPage);
        } catch (error) {
            console.error("Error creating stock:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleEdit = (index) => {
        setEditingStockIndex(index);
    };

    const handleUpdate = async (index) => {
        const token = getJwtToken();
        const stockToUpdate = stocks[index];
        setLoading(true);

        try {
            await fetch(`/api/admin/stock`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    itemId: stockToUpdate.itemId,
                    storeId: stockToUpdate.storeId,
                    numInStock: parseInt(stockToUpdate.numInStock, 10)
                })
            });

            setEditingStockIndex(null);
            fetchStocks(currentPage);
        } catch (error) {
            console.error("Error updating stock:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (index) => {
        const token = getJwtToken();
        const stockToDelete = stocks[index];
        if (window.confirm('Are you sure you want to delete this stock?')) {
            setLoading(true);
            try {
                await fetch(`/api/admin/stock/${stockToDelete.id}`, {
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });
                fetchStocks(currentPage);
            } catch (error) {
                console.error("Error deleting stock:", error);
            } finally {
                setLoading(false);
            }
        }
    };

    const handleChange = (e, index, field) => {
        setStocks(stocks.map((stock, i) => i === index ? { ...stock, [field]: e.target.value } : stock));
    };

    const handlePageChange = (newPage) => {
        if (newPage >= 0 && newPage < totalPages) {
            setCurrentPage(newPage);
            window.history.pushState({}, '', `?tab=${parseInt(new URLSearchParams(window.location.search).get('tab'))}&page=${newPage}`);
        }
    };

    return (
        <div className="container mt-4">
            <h2 className="text-center">Stock Management</h2>

            <form className="row g-2" onSubmit={handleCreate}>
                <div className="col-md-4 col-12">
                    <select
                        className="form-control"
                        value={newStock.itemId}
                        onChange={(e) => setNewStock({ ...newStock, itemId: e.target.value })}
                        required
                    >
                        <option value="" disabled>Select Item</option>
                        {items.map((item) => (
                            <option key={item.id} value={item.id}>{item.name}</option>
                        ))}
                    </select>
                </div>
                <div className="col-md-4 col-12">
                    <select
                        className="form-control"
                        value={newStock.storeId}
                        onChange={(e) => setNewStock({ ...newStock, storeId: e.target.value })}
                        required
                    >
                        <option value="" disabled>Select Store</option>
                        {stores.map((store) => (
                            <option key={store.id} value={store.id}>{store.name}</option>
                        ))}
                    </select>
                </div>
                <div className="col-md-2 col-6">
                    <input
                        type="number"
                        className="form-control"
                        placeholder="Number in Stock"
                        value={newStock.numInStock}
                        min="0"
                        onChange={(e) => setNewStock({ ...newStock, numInStock: e.target.value })}
                        required
                    />
                </div>
                <div className="col-12 text-center">
                    <button type="submit" className="btn btn-primary mt-2" disabled={loading}>Update Stock</button>
                </div>
            </form>

            {loading ? (
                <Spinner />
            ) : (
                <>
                    <ul className="list-group mt-4">
                        {stocks.map((stock, index) => (
                            <li key={index} className="list-group-item">
                                {editingStockIndex === index ? (
                                    <div className="row g-2">
                                        <div className="col-md-4 col-12">
                                            <select
                                                className="form-control"
                                                value={stock.itemId}
                                                onChange={(e) => handleChange(e, index, 'itemId')}
                                            >
                                                {items.map((item) => (
                                                    <option key={item.id} value={item.id}>{item.name}</option>
                                                ))}
                                            </select>
                                        </div>
                                        <div className="col-md-4 col-12">
                                            <select
                                                className="form-control"
                                                value={stock.storeId}
                                                onChange={(e) => handleChange(e, index, 'storeId')}
                                            >
                                                {stores.map((store) => (
                                                    <option key={store.id} value={store.id}>{store.name}</option>
                                                ))}
                                            </select>
                                        </div>
                                        <div className="col-md-2 col-6">
                                            <input
                                                type="number"
                                                className="form-control"
                                                value={stock.numInStock}
                                                min="0"
                                                onChange={(e) => handleChange(e, index, 'numInStock')}
                                            />
                                        </div>
                                        <div className="col-12 text-center">
                                            <button
                                                className="btn btn-success me-2"
                                                onClick={() => handleUpdate(index)}
                                                disabled={loading}>
                                                Save
                                            </button>
                                            <button
                                                className="btn btn-secondary"
                                                onClick={() => setEditingStockIndex(null)}>
                                                Cancel
                                            </button>
                                        </div>
                                    </div>
                                ) : (
                                    <div className="row">
                                        <div className="col-md-8 col-12">
                                            <h4><span title={stock.storeId + "::" + stock.itemId}>🗝</span>Item: {items.find(item => item.id === stock.itemId)?.name || 'N/A'}, Store: {stores.find(store => store.id === stock.storeId)?.name || 'N/A'}</h4>
                                            <p>Number in Stock: {stock.numInStock}</p>
                                        </div>
                                        <div className="col-md-4 col-12 text-center">
                                            <button
                                                className="btn btn-warning me-2"
                                                onClick={() => handleEdit(index)}>
                                                Edit
                                            </button>
                                            <button
                                                className="btn btn-danger"
                                                onClick={() => handleDelete(index)}>
                                                Delete
                                            </button>
                                        </div>
                                    </div>
                                )}
                            </li>
                        ))}
                    </ul>

                    <div className="pagination-controls text-center mt-4">
                        <button
                            onClick={() => handlePageChange(currentPage - 1)}
                            disabled={currentPage === 0}
                            className="btn btn-secondary me-2"
                        >
                            Previous
                        </button>
                        <span>Page {currentPage + 1} of {totalPages}</span>
                        <button
                            onClick={() => handlePageChange(currentPage + 1)}
                            disabled={currentPage === totalPages - 1}
                            className="btn btn-secondary ms-2"
                        >
                            Next
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default Stock;
