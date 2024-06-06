import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { useState } from 'react';
import Home from './components/Home';
import About from './components/About';
import WhatIsThis from './components/WhatIsThis';
import Catalog from './components/Catalog';
import Locations from './components/Locations';
import './App.css';

function App() {

    const [cart, setCart] = useState([]);
    const [total, setTotal] = useState(0);
    const [numItems, setNumItems] = useState(0);

    const addToCart = (item) => {
        setCart([...cart, item]);
        setTotal(total + item.price);
        setNumItems(numItems + 1);
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
                                    <Link className="nav-link" to="/about">About</Link>
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
                            </ul>
                            <div className="dropdown">
                                <button className="btn btn-outline-secondary position-relative">
                                    <div className="shopping-cart-icon">
                                        🛒
                                    </div>
                                        <span className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                                            0
                                            <span className="visually-hidden">items in cart</span>
                                        </span>
                                </button>
                                <ul className="dropdown-menu dropdown-menu-end" aria-labelledby="dropdownCartButton">
                                    <li><a className="dropdown-item" href="#">Cart is empty</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </nav>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/about" element={<About />} />
                    <Route path="/whatisthis" element={<WhatIsThis addToCart={addToCart} />} />
                    <Route path="/catalog" element={<Catalog addToCart={addToCart} />} />
                    <Route path="/locations" element={<Locations />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;
