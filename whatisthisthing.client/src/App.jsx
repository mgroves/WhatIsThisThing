﻿import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { useState, useRef } from 'react';
import Home from './components/Home';
import WhatIsThis from './components/WhatIsThis';
import Catalog from './components/Catalog';
import Stores from './components/Stores';
import CartDropdown from './components/CartDropdown';
import './App.css';
import ProtectedRoute from './components/ProtectedRoute';
import Admin from './components/Admin';
import Login from './components/Login';
import SqlSyntaxModal from './components/SqlSyntaxModal';

function App() {

    const codeRef = useRef(null);



    const [cart, setCart] = useState([]);
    const [total, setTotal] = useState(0);
    const [numItems, setNumItems] = useState(0);
    const [modalBody, setModalBody] = useState("");
    const [modalTitle, setModalTitle] = useState("");

    const modalInfo = (title, body) => {
        setModalTitle(title);
        setModalBody(body);
    };

    const addToCart = (item) => {
        const updatedCart = cart.map(cartItem =>
            cartItem.name === item.name
                ? { ...cartItem, quantity: cartItem.quantity + 1 }
                : cartItem
        );

        if (!updatedCart.find(cartItem => cartItem.name === item.name)) {
            const { image, ...itemWithoutImage } = item; // Exclude the image property
            updatedCart.push({ ...itemWithoutImage, quantity: 1 });
        }

        setCart(updatedCart);
        setTotal(total + item.price);
        setNumItems(numItems + 1);
    };

    const clearCart = () => {
        setCart([]);
        setTotal(0);
        setNumItems(0);
    };


    return (
        <Router>
            <div>
                <nav className="navbar navbar-expand-lg bg-body-tertiary">
                    <div className="container-fluid">
                        <a className="navbar-brand" href="#">What is this?</a>
                        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <div className="collapse navbar-collapse" id="navbarSupportedContent">
                            <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                                <li className="nav-item">
                                    <Link className="nav-link" to="/">Home</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/whatisthis">What is this thing?</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/catalog">Browse Catalog</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/stores">Stores</Link>
                                </li>
                                <CartDropdown cart={cart} total={total} numItems={numItems} clearCart={clearCart} />
                                <li className="nav-item">
                                    <Link className="nav-link" to="/admin">🔐 Admin</Link>
                                </li>
                            </ul>
                            {modalTitle &&
                                <button type="button" className="btn btn-primary" data-bs-toggle="modal" data-bs-target="#exampleModal">
                                    SQL++
                                </button>
                            }
                        </div>

                    </div>
                </nav>
                <Routes>
                    <Route path="/" element={<Home modalInfo={modalInfo} />} />
                    <Route path="/whatisthis" element={<WhatIsThis addToCart={addToCart} modalInfo={modalInfo} />} />
                    <Route path="/catalog" element={<Catalog addToCart={addToCart} modalInfo={modalInfo} />} />
                    <Route path="/stores" element={<Stores modalInfo={modalInfo} />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/admin" element={<ProtectedRoute element={Admin} />} />
                </Routes>
            </div>
            <SqlSyntaxModal modalBody={modalBody} modalTitle={modalTitle} />
        </Router>
    );
}

export default App;
