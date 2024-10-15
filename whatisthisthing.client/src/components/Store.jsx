import React, { forwardRef } from 'react';

const Store = forwardRef(({ store = {} }, ref) => {
    const { name, latitude, longitude } = store;

    // multiple map tile providers that don't require an API key
    const getThunderforestUrl = (latitude, longitude) => {
        const zoom = 16;

        // Helper functions to convert latitude/longitude to tile numbers
        const lon2tile = (longitude, zoom) => Math.floor((longitude + 180) / 360 * Math.pow(2, zoom));
        const lat2tile = (latitude, zoom) => {
            const rad = latitude * Math.PI / 180;
            return Math.floor((1 - Math.log(Math.tan(rad) + 1 / Math.cos(rad)) / Math.PI) / 2 * Math.pow(2, zoom));
        };

        // Calculate tile numbers
        const xTile = lon2tile(longitude, zoom);
        const yTile = lat2tile(latitude, zoom);

        // Construct the Thunderforest URL
        return `https://tile.thunderforest.com/cycle/${zoom}/${xTile}/${yTile}.png`;
    };

    const getWikimediaUrl = (latitude, longitude) => {
        const zoom = 16;

        const lon2tile = (longitude, zoom) => Math.floor((longitude + 180) / 360 * Math.pow(2, zoom));
        const lat2tile = (latitude, zoom) => {
            const rad = latitude * Math.PI / 180;
            return Math.floor((1 - Math.log(Math.tan(rad) + 1 / Math.cos(rad)) / Math.PI) / 2 * Math.pow(2, zoom));
        };

        // Calculate tile numbers
        const xTile = lon2tile(longitude, zoom);
        const yTile = lat2tile(latitude, zoom);

        // Construct the Wikimedia URL
        return `https://maps.wikimedia.org/osm-intl/${zoom}/${xTile}/${yTile}.png`;
    };

    const getUrlCartoDb = (latitude, longitude) => {
        const zoom = 16;

        // convert lon,lat coordinated to carto tile coordinates
        const lon2tile = (longitude, zoom) => Math.floor((longitude + 180) / 360 * Math.pow(2, zoom));
        const lat2tile = (latitude, zoom) => {
            const rad = latitude * Math.PI / 180;
            return Math.floor((1 - Math.log(Math.tan(rad) + 1 / Math.cos(rad)) / Math.PI) / 2 * Math.pow(2, zoom));
        };

        const xTile = lon2tile(longitude, zoom);
        const yTile = lat2tile(latitude, zoom);

        return `https://cartodb-basemaps-a.global.ssl.fastly.net/rastertiles/voyager/${zoom}/${xTile}/${yTile}.png`;
    };

    const getUrlYandex = (latitude, longitude) => {
        return `https://static-maps.yandex.ru/1.x/?ll=${longitude},${latitude}&z=15&l=map&size=450,450&pt=${longitude},${latitude},pm2rdm`;
    };

    const imageUrl = getUrlCartoDb(latitude, longitude)

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
