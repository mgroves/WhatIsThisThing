import React, { useId, forwardRef } from 'react';
import StoreAvailability from './StoreAvailability';

const Item = forwardRef(({ item = {}, addToCart }, ref) => {
    const { image, name, desc, price, stock = [] } = item;
    const accordionId = useId();

    return (
        <div className="card" style={{ width: '18rem' }} ref={ref}>
            <img className="card-img-top" src={image} alt={name} />
            <div className="card-body">
                <h5 className="card-title">{name}</h5>
                <p className="card-text">{desc}</p>
                <p className="card-text">${price}</p>
                <div className="accordion" id={accordionId}>
                    <div className="accordion-item">
                        <h2 className="accordion-header" id={`${accordionId}-headingOne`}>
                            <button className="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target={`#${accordionId}-collapseOne`} aria-expanded="true" aria-controls={`${accordionId}-collapseOne`}>
                                Available Nearby
                            </button>
                        </h2>
                        <div id={`${accordionId}-collapseOne`} className="accordion-collapse collapse" aria-labelledby={`${accordionId}-headingOne`} data-bs-parent={`#${accordionId}`}>
                            <div className="accordion-body">
                                {stock.length === 0 ? ( <p>No locations near you.</p> ) : (
                                    stock.map((store, index) => (
                                        <StoreAvailability key={index} store={store} />
                                    ))
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div className="card-footer">
                <button className="btn btn-success" onClick={() => addToCart(item)}>Add to Cart</button>
            </div>
        </div>
    );
});

export default Item;
