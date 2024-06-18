import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { useState } from 'react';
import Home from './components/Home';
import WhatIsThis from './components/WhatIsThis';
import Catalog from './components/Catalog';
import Locations from './components/Locations';
import CartDropdown from './components/CartDropdown';
import './App.css';

function App() {

    const [cart, setCart] = useState([]);
    const [total, setTotal] = useState(0);
    const [numItems, setNumItems] = useState(0);

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
                                    <Link className="nav-link" to="/locations">Locations</Link>
                                </li>
                                <CartDropdown cart={cart} total={total} numItems={numItems} clearCart={clearCart} />
                            </ul>
                        </div>

                    </div>
                </nav>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/whatisthis" element={<WhatIsThis addToCart={addToCart} />} />
                    <Route path="/catalog" element={<Catalog addToCart={addToCart} />} />
                    <Route path="/locations" element={<Locations />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;
