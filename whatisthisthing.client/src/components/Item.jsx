import React, { useId, forwardRef } from 'react';
import StoreAvailability from './StoreAvailability';

const Item = forwardRef(({ item = {}, addToCart }, ref) => {
    const { image, name, desc, price, stock = [], score } = item;
    const accordionId = useId();

    const getProgressBarColor = (score) => {
        if (score > 0.005) return 'bg-success'; // great match (green)
        if (score > 0.0005) return 'bg-warning'; // good match (yellow)
        return 'bg-danger'; // not a match (red)
    };

    const getScorePercentage = (score) => {
        // Use an adjusted logarithmic scale for normalization
        const logScore = Math.log10(score + 1); // Adding 1 to avoid log(0)
        const adjustedScore = (logScore / Math.log10(0.01 + 1)) * 100; // Normalizing against the maximum expected score

        // Adjusting the scaling to ensure better fill for yellow and red
        if (score > 0.005) {
            return Math.min(adjustedScore + 20, 100); // Boost for green matches
        } else if (score > 0.0005) {
            return Math.min(adjustedScore * 20, 100); // Boost for yellow matches
        } else {
            return Math.min(adjustedScore * 20, 100); // Boost for red matches
        }
    };

    const roundScore = (score) => {
        return score.toFixed(3); // Adjust the number of decimal places as needed
    };

    const progressBarColor = getProgressBarColor(score);
    const scorePercentage = getScorePercentage(score);
    const roundedScore = roundScore(score);

    return (
        <div className="card" style={{ width: '18rem' }} ref={ref}>
            <img className="card-img-top" src={image} alt={name} />
            <div className="card-body">
                <h5 className="card-title">{name}</h5>
                <p className="card-text">{desc} ({roundedScore})</p>
                <div className="progress" role="progressbar" aria-label="Basic example" aria-valuenow={scorePercentage} aria-valuemin="0" aria-valuemax="100">
                    <div className={`progress-bar ${progressBarColor}`} style={{ width: `${scorePercentage}%`, position: 'relative' }}>
                        <span style={{ position: 'absolute', width: '100%', color: '#000', textAlign: 'center' }}>{score}</span>
                    </div>
                </div>
                <p className="card-text">${price}</p>
            </div>
            <div className="card-footer">
                <div className="accordion" id={accordionId}>
                    <div className="accordion-item">
                        <h2 className="accordion-header" id={`${accordionId}-headingOne`}>
                            <button className="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target={`#${accordionId}-collapseOne`} aria-expanded="true" aria-controls={`${accordionId}-collapseOne`}>
                                Available Nearby
                            </button>
                        </h2>
                        <div id={`${accordionId}-collapseOne`} className="accordion-collapse collapse" aria-labelledby={`${accordionId}-headingOne`} data-bs-parent={`#${accordionId}`}>
                            <div className="accordion-body">
                                {stock.length === 0 ? (<p>No locations near you.</p>) : (
                                    stock.map((store, index) => (
                                        <StoreAvailability key={index} store={store} />
                                    ))
                                )}
                            </div>
                        </div>
                    </div>
                </div>
                <button className="btn btn-success" onClick={() => addToCart(item)}>Add to Cart</button>
            </div>
        </div>
    );
});

export default Item;
