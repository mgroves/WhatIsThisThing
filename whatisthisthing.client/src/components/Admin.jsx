import React, { useState } from 'react';
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

    const [activeTab, setActiveTab] = useState(0);

    return (
        <div>
            {/* Nav Tabs */}
            <ul className="nav nav-tabs">
                {tabs.map((tab, index) => (
                    <li className="nav-item" key={index}>
                        <a
                            className={`nav-link ${activeTab === index ? 'active' : ''}`}
                            href="#"
                            onClick={() => setActiveTab(index)}
                        >
                            {tab.name}
                        </a>
                    </li>
                ))}
            </ul>

            {/* Tab Content */}
            <div className="tab-content mt-3">
                <div className="tab-pane fade show active">
                    {tabs[activeTab].component}
                </div>
            </div>
        </div>
    );
};

export default Admin;
