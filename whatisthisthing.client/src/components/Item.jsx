import React from 'react';
import StoreAvailability from './StoreAvailability';

const Item = ({ item = {}, addToCart }) => {
    const { image, name, desc, price, stock = [] } = item;
    return (
        <div className="card" style={{ width: '18rem' }}>
            <img className="card-img-top" src={image} alt={name} />
            <div className="card-body">
                <h5 className="card-title">{name}</h5>
                <p className="card-text">{desc}</p>
                <p className="card-text">${price}</p>
                <div className="accordion">
                    <div className="accordion-item">
                        <h2 className="accordion-header">
                            <button className="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                Available Nearby
                            </button>
                        </h2>
                        <div id="collapseOne" className="accordion-collapse collapse" data-bs-parent="#accordionExample">
                            <div className="accordion-body">
                                {stock.map((store, index) => (
                                    <StoreAvailability key={index} store={store} />
                                ))}
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div className="card-footer">
                <small className="text-body-secondary">
                    <button className="btn btn-success" onClick={() => addToCart(item)}>Add to Cart</button>
                </small>
            </div>
        </div>
    );
};

export default Item;
