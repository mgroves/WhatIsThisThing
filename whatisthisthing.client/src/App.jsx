import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import Home from './components/Home';
import About from './components/About';
import WhatIsThis from './components/WhatIsThis';
import Catalog from './components/Catalog';
import Locations from './components/Locations';
import './App.css';

function App() {
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
                        </div>
                    </div>
                </nav>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/about" element={<About />} />
                    <Route path="/whatisthis" element={<WhatIsThis />} />
                    <Route path="/catalog" element={<Catalog />} />
                    <Route path="/locations" element={<Locations />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;
