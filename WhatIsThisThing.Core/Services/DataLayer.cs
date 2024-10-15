using Couchbase.Extensions.DependencyInjection;
using WhatIsThisThing.Core.Domain;
using WhatIsThisThing.Core.Request;
using WhatIsThisThing.Core.Response;

namespace WhatIsThisThing.Core.Services;

public interface IDataLayer
{
    Task<WithModalInfo<List<ItemResponse>>> FindItemsWithStockByVectorAndLocation(float[] embedding, GeoCoord requestLocation);
    Task<WithModalInfo<List<ItemResponse>>> BrowseCatalog(BrowseRequest request);
    Task<WithModalInfo<List<Store>>> GetStores(LocationsRequest request);
}

public class DataLayer : IDataLayer
{
    private const int PAGE_SIZE = 6;
    private const string RADIUS = "15mi";
    private readonly IBucketProvider _bucketProvider;

    public DataLayer(IBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }
    
    public async Task<WithModalInfo<List<ItemResponse>>> FindItemsWithStockByVectorAndLocation(float[] embedding, GeoCoord requestLocation)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;

        var sql = @$"
            WITH closestStores AS (                     /* CTE to get closest stores based on user's location */
                SELECT x.name, META(x).id AS id
                FROM whatisthis._default.Stores x
                WHERE SEARCH(x, {{
                 ""fields"": [""*""],
                 ""query"" : {{
                   ""location"" : {{
                     ""lat"" : {requestLocation.Latitude},
                     ""lon"" : {requestLocation.Longitude}
                   }},
                   ""distance"" : ""{RADIUS}"",
                   ""field"" : ""geo""
                 }}, 
                 ""sort"": [
                  {{
                   ""by"": ""geo_distance"",
                   ""field"": ""geo"",
                   ""unit"": ""mi"",
                   ""location"" : {{
                     ""lat"" : {requestLocation.Latitude},
                     ""lon"" : {requestLocation.Longitude}
                   }}
                  }}
                 ]
	            }})

	            LIMIT 3
            )
            /* SELECT items with nearby stock numbers */

            SELECT allItems.name, allItems.`desc`, allItems.image, allItems.price, allItems.rating, SEARCH_SCORE(allItems) AS score,

                /* subquery to get stock from nearby locations */

                (SELECT closestStores.name AS storeName, stock.numInStock AS quantity
                 FROM whatisthis._default.Items items
                 JOIN whatisthis._default.Stock stock
                    ON SPLIT(META(stock).id,""::"")[1] == META(items).id
                 JOIN closestStores
                    ON SPLIT(META(stock).id,""::"")[0] == closestStores.id
                 WHERE META(items).id = META(allItems).id) AS stock

            FROM whatisthis._default.Items AS allItems

            WHERE SEARCH(allItems,                    /* vector search using image embedding */
              {{
                ""fields"": [""*""],
                ""query"": {{
                  ""match_none"": {{}}
                }},
                ""knn"": [
                  {{
                    ""k"": 4,
                    ""field"": ""imageVector"",
                    ""vector"": [ {string.Join(',', embedding)} ]
                  }}
                ]
              }}
            )
            ORDER BY score DESC";

        var result = await cluster.QueryAsync<ItemResponse>(sql);
        var rows = result.Rows.AsAsyncEnumerable();
        return new WithModalInfo<List<ItemResponse>>
        {
            ModalTitle = "Find items with stock by vector and location",
            ModalContent = sql,
            Data = await rows.ToListAsync()
        };
    }

    public async Task<WithModalInfo<List<ItemResponse>>> BrowseCatalog(BrowseRequest request)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;

        var sql = @$"
            WITH closestStores AS (                     /* CTE to find 3 closet stores to user */
                SELECT x.name, META(x).id AS id
                FROM whatisthis._default.Stores x
                WHERE SEARCH(x, {{                      /* Using FTS engine for radius search */
                 ""fields"": [""*""],
                 ""query"" : {{
                   ""location"" : {{
                     ""lat"" : {request.Latitude},
                     ""lon"" : {request.Longitude}
                   }},
                   ""distance"" : ""{RADIUS}"",
                   ""field"" : ""geo""
                 }}, 
                 ""sort"": [
                  {{
                   ""by"": ""geo_distance"",
                   ""field"": ""geo"",
                   ""unit"": ""mi"",
                   ""location"" : {{
                     ""lat"" : {request.Latitude},
                     ""lon"" : {request.Longitude}
                   }}
                  }}
                 ]
	            }})

	            LIMIT 3
            )

            /* SELECT items with nearby stock numbers */

            SELECT allItems.name, allItems.`desc`, allItems.image, allItems.price, allItems.rating,

                /* subquery to get stock from nearby locations */

	            (SELECT closestStores.name AS storeName, stock.numInStock AS quantity
	             FROM whatisthis._default.Items items
	             JOIN whatisthis._default.Stock stock
		            ON SPLIT(META(stock).id,""::"")[1] == META(items).id
	             JOIN closestStores
		            ON SPLIT(META(stock).id,""::"")[0] == closestStores.id
	             WHERE META(items).id = META(allItems).id) AS stock

            FROM whatisthis._default.Items AS allItems
            WHERE 1==1
                {WhereMinPrice(request)}            /* min price filter (if any) */
                {WhereMaxPrice(request)}            /* max price filter (if any) */
                {WhereMinRating(request)}           /* min rating filter (if any) */

            /* ordering by name for pagination */

            ORDER BY u.name
            LIMIT {PAGE_SIZE}
            OFFSET {request.Page * PAGE_SIZE}";

        var result = await cluster.QueryAsync<ItemResponse>(sql);
        var rows = result.Rows.AsAsyncEnumerable();
        return new WithModalInfo<List<ItemResponse>>
        {
            ModalTitle = "Browse catalog (paged)",
            ModalContent = sql,
            Data = await rows.ToListAsync()
        };
    }

    private string WhereMinRating(BrowseRequest request)
    {
        if (!request.MinRating.HasValue)
            return string.Empty;
        if (request.MinRating is < 0 or > 5)
            return string.Empty;
        return $" AND allItems.rating >= {request.MinRating}";
    }

    private string WhereMaxPrice(BrowseRequest request)
    {
        if (!request.MaxPrice.HasValue)
            return string.Empty;
        return $" AND allItems.price <= {request.MaxPrice} ";
    }

    private string WhereMinPrice(BrowseRequest request)
    {
        if (!request.MinPrice.HasValue)
            return string.Empty;
        return $" AND allItems.price >= {request.MinPrice} ";
    }

    public async Task<WithModalInfo<List<Store>>> GetStores(LocationsRequest request)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;

        var sql = @$"
            SELECT l.name, l.geo.lat AS Latitude, l.geo.lon AS Longitude
            FROM whatisthis._default.Stores l
            LIMIT {PAGE_SIZE} 
            OFFSET {request.Page * PAGE_SIZE}";

        var result = await cluster.QueryAsync<Store>(sql);
        var rows = result.Rows.AsAsyncEnumerable();
        return new WithModalInfo<List<Store>>
        {
            ModalTitle = "Browse stores (paged)",
            ModalContent = sql,
            Data = await rows.ToListAsync()
        };
    }
}