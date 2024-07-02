import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { useState, useRef } from 'react';
import Home from './components/Home';
import WhatIsThis from './components/WhatIsThis';
import Catalog from './components/Catalog';
import Stores from './components/Stores';
import CartDropdown from './components/CartDropdown';
import './App.css';

function App() {

    const codeRef = useRef(null);

    const handleCopy = () => {
        const codeContent = codeRef.current.innerText;
        navigator.clipboard.writeText(codeContent).then(() => {
            alert('Copied to clipboard!');
        }).catch(err => {
            console.error('Failed to copy!', err);
        });
    };

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
                </Routes>
            </div>
            <div className="modal fade" id="exampleModal" tabIndex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog modal-xl">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5" id="exampleModalLabel">{modalTitle}</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div className="modal-body">
                            <pre className="">
                                <code ref={codeRef}>
                                    {modalBody}
                                </code>
                            </pre>
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">✖Close</button>
                            <button type="button" className="btn btn-primary" onClick={handleCopy}>📋Copy</button>
                        </div>
                    </div>
                </div>
            </div>
        </Router>
    );
}

export default App;
