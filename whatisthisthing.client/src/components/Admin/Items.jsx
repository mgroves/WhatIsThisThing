import React, { useState, useEffect } from 'react';
import Spinner from '../Spinner'

const Items = () => {
    const [items, setItems] = useState([]);
    const [newItem, setNewItem] = useState({ name: '', desc: '', price: '', rating: '', image: '' });
    const [editingItemId, setEditingItemId] = useState(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        fetchItems();
    }, []);

    const getJwtToken = () => {
        const authData = JSON.parse(localStorage.getItem('wittAuth'));
        return authData ? authData.token.jwtToken : null;
    };

    const fetchItems = async () => {
        setLoading(true); // Start spinner
        const token = getJwtToken();
        const response = await fetch('/api/items', {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });
        const data = await response.json();
        setItems(data);
        setLoading(false); // Stop spinner
    };

    const handleCreate = async (e) => {
        e.preventDefault();
        setLoading(true);

        const fileInput = document.getElementById("create-image-input");
        const file = fileInput.files[0];

        if (file) {
            const reader = new FileReader();
            reader.onloadend = async () => {
                const base64String = reader.result;
                const itemToCreate = { ...newItem, image: base64String };
                await submitNewItem(itemToCreate);
            };
            reader.readAsDataURL(file);
        } else {
            await submitNewItem(newItem);
        }
    };

    const submitNewItem = async (item) => {
        const token = getJwtToken();
        await fetch('/api/items', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(item)
        });
        setNewItem({ name: '', desc: '', price: '', rating: '', image: '' });
        fetchItems();
        setLoading(false);
    };

    const handleEdit = (item) => {
        setEditingItemId(item.id);
    };

    const handleUpdate = async (id) => {
        const token = getJwtToken();
        const itemToUpdate = items.find(item => item.id === id);
        setLoading(true);

        await fetch(`/api/items/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(itemToUpdate)
        });

        setEditingItemId(null);
        fetchItems();
        setLoading(false);
    };

    const handleDelete = async (id) => {
        const token = getJwtToken();
        if (window.confirm('Are you sure you want to delete this item?')) {
            await fetch(`/api/items/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            fetchItems();
        }
    };

    const handleFileChange = (e, id = null) => {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onloadend = () => {
                if (id) {
                    setItems(items.map(item => item.id === id ? { ...item, image: reader.result } : item));
                } else {
                    setNewItem({ ...newItem, image: reader.result });
                }
            };
            reader.readAsDataURL(file);
        }
    };

    const handleChange = (e, id, field) => {
        setItems(items.map(item => item.id === id ? { ...item, [field]: e.target.value } : item));
    };

    return (
        <div className="container mt-4">
            <h2 className="text-center">Items</h2>

            <form className="row g-2" onSubmit={handleCreate}>
                <div className="col-md-3 col-6">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Name"
                        value={newItem.name}
                        onChange={(e) => setNewItem({ ...newItem, name: e.target.value })}
                        required
                    />
                </div>
                <div className="col-md-3 col-6">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Description"
                        value={newItem.desc}
                        onChange={(e) => setNewItem({ ...newItem, desc: e.target.value })}
                        required
                    />
                </div>
                <div className="col-md-2 col-6">
                    <input
                        type="number"
                        className="form-control"
                        placeholder="Price"
                        value={newItem.price}
                        onChange={(e) => setNewItem({ ...newItem, price: e.target.value })}
                        required
                    />
                </div>
                <div className="col-md-2 col-6">
                    <input
                        type="number"
                        className="form-control"
                        placeholder="Rating"
                        value={newItem.rating}
                        onChange={(e) => setNewItem({ ...newItem, rating: e.target.value })}
                        required
                    />
                </div>
                <div className="col-md-2 col-12">
                    <input
                        id="create-image-input"
                        type="file"
                        className="form-control"
                        onChange={(e) => handleFileChange(e)}
                        required
                    />
                </div>
                <div className="col-12 text-center">
                    <button type="submit" className="btn btn-primary mt-2" disabled={loading}>Create Item</button>
                </div>
            </form>

            {loading ? (
                <Spinner />  // Show Spinner when loading
            ) : (<>

            <ul className="list-group mt-4">
                {items.map(item => (
                    <li key={item.id} className="list-group-item">
                        {editingItemId === item.id ? (
                            <div className="row g-2">
                                <div className="col-md-3 col-6">
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={item.name}
                                        onChange={(e) => handleChange(e, item.id, 'name')}
                                    />
                                </div>
                                <div className="col-md-3 col-6">
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={item.desc}
                                        onChange={(e) => handleChange(e, item.id, 'desc')}
                                    />
                                </div>
                                <div className="col-md-2 col-6">
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={item.price}
                                        onChange={(e) => handleChange(e, item.id, 'price')}
                                    />
                                </div>
                                <div className="col-md-2 col-6">
                                    <input
                                        type="number"
                                        className="form-control"
                                        value={item.rating}
                                        onChange={(e) => handleChange(e, item.id, 'rating')}
                                    />
                                </div>
                                <div className="col-md-2 col-12">
                                    <input
                                        type="file"
                                        className="form-control"
                                        onChange={(e) => handleFileChange(e, item.id)}
                                    />
                                </div>
                                <div className="col-12 text-center">
                                    <button
                                        className="btn btn-success me-2"
                                        onClick={() => handleUpdate(item.id)}
                                        disabled={loading}>
                                        Save
                                    </button>
                                    <button
                                        className="btn btn-secondary"
                                        onClick={() => setEditingItemId(null)}>
                                        Cancel
                                    </button>
                                </div>
                            </div>
                        ) : (
                            <div className="row">
                                <div className="col-md-2 col-12">
                                    <img src={item.image} alt={item.name} className="img-fluid" style={{ height: '64px' }} />
                                </div>
                                <div className="col-md-8 col-12">
                                    <h4>{item.name}</h4>
                                    <p>{item.desc}</p>
                                </div>
                                <div className="col-md-2 col-12 text-center">
                                    <button
                                        className="btn btn-warning me-2"
                                        onClick={() => handleEdit(item)}>
                                        Edit
                                    </button>
                                    <button
                                        className="btn btn-danger"
                                        onClick={() => handleDelete(item.id)}>
                                        Delete
                                    </button>
                                </div>
                            </div>
                        )}
                    </li>
                ))}
                    </ul>
            </>
            )}
        </div>
    );

};

export default Items;
