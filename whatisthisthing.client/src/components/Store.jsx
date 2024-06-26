import React, { forwardRef } from 'react';

const Store = forwardRef(({ store = {} }, ref) => {
    const { name, latitude, longitude } = store;

    // Construct the OpenStreetMap static map URL
    const imageUrl = `https://static-maps.yandex.ru/1.x/?ll=${longitude},${latitude}&z=15&l=map&size=450,450&pt=${longitude},${latitude},pm2rdm`;
    const googleMapsUrl = `https://www.google.com/maps?q=${latitude},${longitude}`;

    return (
        <div className="card" style={{ width: '18rem' }} ref={ref}>
            <img className="card-img-top" src={imageUrl} alt={name} />
            <div className="card-body">
                <h5 className="card-title">{name}</h5>
                <p className="card-text">Latitude: {latitude}</p>
                <p className="card-text">Longitude: {longitude}</p>
                <a href={googleMapsUrl} target="_blank" rel="noopener noreferrer" className="btn btn-primary">
                    View on Google Maps
                </a>
            </div>
        </div>
    );
});

export default Store;
