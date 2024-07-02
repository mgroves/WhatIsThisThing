import React, { useEffect } from 'react';

function Home({ modalInfo }) {
    useEffect(() => {
        modalInfo("", "");
    });

    return (
        <main>

            <section className="py-5 text-center container">
                <div className="row">
                    <div className="col-lg-6 col-md-8 mx-auto">
                        <h1 className="fw-light">What Is This Thing?</h1>
                        <p className="lead text-body-secondary"><strong>What Is This Thing?</strong> is a demo application showcasing vector search
                            and hybrid search using Couchbase. It is a demo eCommerce website with
                            features like semantic visual search with vectors, geo-location-based product availability, and more.</p>
                        <p>
                            <a href="https://www.couchbase.com/products/capella/" className="btn btn-primary my-2">Get Started for Free with Capella</a>
                            <a href="https://www.couchbase.com/downloads/" className="btn btn-secondary my-2">Download Couchbase Server</a>
                        </p>
                    </div>
                </div>
            </section>

            <div className="album py-5 bg-body-tertiary">
                <div className="container">
                    <div class="card-group">
                        <div class="card">
                            <img src="images/visualsearch.jpg" className="card-img-top" />
                                <div class="card-body">
                                <h5 class="card-title">Semantic visual search</h5>
                                <p class="card-text">Couchbase supports knn (nearest neighbor) search on vector embeddings. With an image model, this means you can search for visually similar images.</p>
                                </div>
                                <div class="card-footer">
                                    <a class="btn btn-primary" href="/whatisthis" role="button">Try it now</a>
                                </div>
                        </div>
                        <div class="card">
                            <img src="images/geospatial.jpg" className="card-img-top" />
                                <div class="card-body">
                                <h5 class="card-title">Geospatial Search</h5>
                                <p class="card-text">Couchbase supports search by geolocation (latitude and longitude). This allows searches for data that is within distances, bounding polygons, radius, etc.</p>
                                </div>
                                <div class="card-footer">
                                <a class="btn btn-primary" href="/catalog" role="button">Try it now</a>
                                </div>
                        </div>
                        <div class="card">
                            <img src="images/hybrid.jpg" className="card-img-top" />
                                <div class="card-body">
                                <h5 class="card-title">SQL++ Hybrid Search</h5>
                                <p class="card-text">Couchbase's query language is SQL++, supporting standard SQL predicates, as well as vector, location, time series, and more, all from a single query.</p>
                                </div>
                                <div class="card-footer">
                                <a class="btn btn-primary" href="https://docs.couchbase.com/server/current/search/run-searches.html#sql" role="button">Learn More</a>
                                </div>
                        </div>
                    </div>
                </div>
            </div>

        </main>

        
    );
}

export default Home;
