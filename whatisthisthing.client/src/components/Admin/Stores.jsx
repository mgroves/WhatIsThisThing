import React, { useState, useEffect } from 'react';
import Spinner from '../Spinner';

const Stores = () => {
    const [stores, setStores] = useState([]);
    const [newStore, setNewStore] = useState({ name: '', latitude: '', longitude: '' });
    const [editingStoreIndex, setEditingStoreIndex] = useState(null);
    const [loading, setLoading] = useState(false);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    useEffect(() => {
        const params = new URLSearchParams(window.location.search);
        const page = parseInt(params.get('page')) || 0;
        setCurrentPage(page);
        fetchStores(page);
    }, [currentPage]);

    const getJwtToken = () => {
        const authData = JSON.parse(localStorage.getItem('wittAuth'));
        return authData ? authData.token.jwtToken : null;
    };

    const fetchStores = async (page = 0) => {
        setLoading(true);
        const token = getJwtToken();
        const response = await fetch(`/api/admin/stores?page=${page}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        const data = await response.json();
        setStores(data.items);
        setTotalPages(data.totalPages);
        setLoading(false);
    };

    const handlePageChange = (newPage) => {
        setCurrentPage(newPage);
        window.history.pushState({}, '', `?page=${newPage}`);
        fetchStores(newPage);
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        setLoading(true);
        await submitNewStore(newStore);
    };

    const submitNewStore = async (store) => {
        const token = getJwtToken();
        await fetch('/api/admin/stores', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                name: store.name,
                latitude: parseFloat(store.latitude),
                longitude: parseFloat(store.longitude)
            })
        });
        setNewStore({ name: '', latitude: '', longitude: '' });
        fetchStores();
        setLoading(false);
    };

    const handleEdit = (index) => {
        setEditingStoreIndex(index);
    };

    const handleUpdate = async (index) => {
        const token = getJwtToken();
        const storeToUpdate = stores[index];
        setLoading(true);

        await fetch(`/api/admin/stores/${storeToUpdate.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                name: storeToUpdate.name,
                latitude: parseFloat(storeToUpdate.latitude),
                longitude: parseFloat(storeToUpdate.longitude)
            })
        });

        setEditingStoreIndex(null);
        fetchStores();
        setLoading(false);
    };

    const handleDelete = async (index) => {
        const token = getJwtToken();
        const storeToDelete = stores[index];
        if (window.confirm('Are you sure you want to delete this store?')) {
            await fetch(`/api/admin/stores/${storeToDelete.id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            fetchStores();
        }
    };

    const handleChange = (e, index, field) => {
        setStores(stores.map((store, i) => i === index ? { ...store, [field]: e.target.value } : store));
    };

    return (
        <div className="container mt-4">
            <h2 className="text-center">Stores</h2>

            <form className="row g-2" onSubmit={handleCreate}>
                <div className="col-md-4 col-12">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Store Name"
                        value={newStore.name}
                        onChange={(e) => setNewStore({ ...newStore, name: e.target.value })}
                        required
                    />
                </div>
                <div className="col-md-2 col-6">
                    <input
                        type="number"
                        className="form-control"
                        placeholder="Latitude"
                        value={newStore.latitude}
                        min="-90"
                        max="90"
                        onChange={(e) => setNewStore({ ...newStore, latitude: e.target.value })}
                        required
                    />
                </div>
                <div className="col-md-2 col-6">
                    <input
                        type="number"
                        className="form-control"
                        placeholder="Longitude"
                        value={newStore.longitude}
                        min="-180"
                        max="180"
                        onChange={(e) => setNewStore({ ...newStore, longitude: e.target.value })}
                        required
                    />
                </div>
                <div className="col-12 text-center">
                    <button type="submit" className="btn btn-primary mt-2" disabled={loading}>Create Store</button>
                </div>
            </form>

            {loading ? (
                <Spinner />
            ) : (
                <>
                    <ul className="list-group mt-4">
                        {stores.map((store, index) => (
                            <li key={index} className="list-group-item">
                                {editingStoreIndex === index ? (
                                    <div className="row g-2">
                                        <div className="col-md-4 col-12">
                                            <input
                                                type="text"
                                                className="form-control"
                                                value={store.name}
                                                onChange={(e) => handleChange(e, index, 'name')}
                                            />
                                        </div>
                                        <div className="col-md-2 col-6">
                                            <input
                                                type="number"
                                                className="form-control"
                                                value={store.latitude}
                                                min="-90"
                                                max="90"
                                                onChange={(e) => handleChange(e, index, 'latitude')}
                                            />
                                        </div>
                                        <div className="col-md-2 col-6">
                                            <input
                                                type="number"
                                                className="form-control"
                                                value={store.longitude}
                                                min="-180"
                                                max="180"
                                                onChange={(e) => handleChange(e, index, 'longitude')}
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
                                                onClick={() => setEditingStoreIndex(null)}>
                                                Cancel
                                            </button>
                                        </div>
                                    </div>
                                ) : (
                                    <div className="row">
                                        <div className="col-md-8 col-12">
                                            <h4>{store.name}</h4>
                                            <p>Latitude: {store.latitude}, Longitude: {store.longitude}</p>
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
                            disabled={currentPage === totalPages}
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

export default Stores;
