import React from 'react';

function Home() {
    return (
        <div className="container mt-5">
            <div className="text-center">
                <h2 className="display-4">Welcome to "What Is This Thing?"</h2>
                <p className="lead"><strong>What Is This Thing?</strong> is a demo application showcasing vector search
                    and hybrid search using Couchbase. It is a demo eCommerce website with
                    features like semantic visual search with vectors, geo-location-based product availability, and more.</p>
            </div>
            <div id="carouselExample" className="carousel slide">
                <div className="carousel-indicators">
                    <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                    <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
                    <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
                </div>
                <div className="carousel-inner">
                    <div className="carousel-item active">
                        <img src="https://picsum.photos/1024/300?random=1" className="d-block w-100" />
                        <div className="carousel-caption d-none d-md-block text-shadow">
                            <h5>Semantic visual search</h5>
                            <p>Couchbase supports knn (nearest neighbor) search on vector embeddings. With an image model, this means you can search for visually similar images.</p>
                        </div>
                    </div>
                    <div className="carousel-item">
                        <img src="https://picsum.photos/1024/300?random=2" className="d-block w-100" />
                        <div className="carousel-caption d-none d-md-block text-shadow">
                            <h5>Geospatial Search</h5>
                            <p>Couchbase also supports search by geolocation (latitude and longitude). This allows searches for data that is within distances, bounding polygons, radius, etc.</p>
                        </div>
                    </div>
                    <div className="carousel-item">
                        <img src="https://picsum.photos/1024/300?random=3" className="d-block w-100" />
                        <div className="carousel-caption d-none d-md-block text-shadow">
                            <h5>SQL++ Hybrid Search</h5>
                            <p>Couchbase's query language is SQL++, supporting standard SQL predicates, as well as vector, location, time series, and more, all from a single query.</p>
                        </div>
                    </div>
                </div>
                <button className="carousel-control-prev" type="button" data-bs-target="#carouselExample" data-bs-slide="prev">
                    <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                    <span className="visually-hidden">Previous</span>
                </button>
                <button className="carousel-control-next" type="button" data-bs-target="#carouselExample" data-bs-slide="next">
                    <span className="carousel-control-next-icon" aria-hidden="true"></span>
                    <span className="visually-hidden">Next</span>
                </button>
            </div>
        </div>
    );
}

export default Home;
