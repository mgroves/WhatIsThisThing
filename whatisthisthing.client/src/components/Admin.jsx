import React, { useState, useEffect } from 'react';
import Stores from "./Admin/Stores";
import Stock from "./Admin/Stock";
import Items from "./Admin/Items";

const Admin = () => {
    // Define tabs
    const tabs = [
        { name: 'Stores', component: <Stores /> },
        { name: 'Items', component: <Items /> },
        { name: 'Stock', component: <Stock /> },
    ];

    // Get initial tab and page from URL
    const params = new URLSearchParams(window.location.search);
    const initialTab = parseInt(params.get('tab')) || 0;
    const initialPage = parseInt(params.get('page')) || 0;

    const [activeTab, setActiveTab] = useState(initialTab);
    const [currentPage, setCurrentPage] = useState(initialPage);

    const handleTabChange = (index) => {
        setActiveTab(index);
        setCurrentPage(0); // Reset page number when switching tabs
        window.history.pushState({}, '', `?tab=${index}`);
    };

    useEffect(() => {
        const params = new URLSearchParams(window.location.search);
        const tab = parseInt(params.get('tab')) || 0;
        const page = parseInt(params.get('page')) || 0;
        setActiveTab(tab);
        setCurrentPage(page);
    }, []);

    return (
        <div>
            {/* Nav Tabs */}
            <ul className="nav nav-tabs">
                {tabs.map((tab, index) => (
                    <li className="nav-item" key={index}>
                        <a
                            className={`nav-link ${activeTab === index ? 'active' : ''}`}
                            href="javascript:;"
                            role="tab"
                            tabIndex={0}
                            onClick={() => handleTabChange(index)}
                        >
                            {tab.name}
                        </a>
                    </li>
                ))}
            </ul>

            {/* Tab Content */}
            <div className="tab-content mt-3">
                <div className="tab-pane fade show active">
                    {React.cloneElement(tabs[activeTab].component, { currentPage, setCurrentPage })}
                </div>
            </div>
        </div>
    );
};

export default Admin;
