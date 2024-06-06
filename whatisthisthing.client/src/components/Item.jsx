import React from 'react';
import StoreAvailability from './StoreAvailability';


const Item = ({ item, addToCart }) => (
    <div className="card" style={{ width: '18rem' }}>
        <img className="card-img-top" src={item.image} alt={item.name} />
        <div className="card-body">
            <h5 className="card-title">{item.name}</h5>
            <p className="card-text">{item.desc}</p>
            <p className="card-text">${item.price}</p>
            <div className="accordion">
                <div className="accordion-item">
                    <h2 className="accordion-header">
                        <button className="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                            Available Nearby
                        </button>
                    </h2>
                    <div id="collapseOne" className="accordion-collapse collapse" data-bs-parent="#accordionExample">
                        <div className="accordion-body">
                            {item.stock.map((store, index) => (
                                <StoreAvailability key={index} store={store} />
                            ))}
                        </div>
                    </div>
                </div>
            </div>
            <button className="btn btn-success" onClick={() => addToCart(item)}>Add to Cart</button>
        </div>
    </div>
);

export default Item;
