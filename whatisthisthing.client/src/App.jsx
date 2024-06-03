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
                <nav>
                    <ul>
                        <li>
                            <Link to="/">Home</Link>
                        </li>
                        <li>
                            <Link to="/about">About</Link>
                        </li>
                        <li>
                            <Link to="/whatisthis">What is this thing?</Link>
                        </li>
                        <li>
                            <Link to="/catalog">Browse Catalog</Link>
                        </li>
                        <li>
                            <Link to="/locations">Locations</Link>
                        </li>
                    </ul>
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
