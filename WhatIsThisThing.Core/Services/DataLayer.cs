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
            SELECT t1.name, t1.`desc`, t1.image, t1.price, SEARCH_SCORE(t1) AS score,
                ARRAY_AGG({{
                    ""storeName"": store1.name,
                    ""quantity"": stock1.numInStock
                }}) AS Stock
            FROM whatisthis._default.Items AS t1
            LEFT JOIN whatisthis._default.Stock AS stock1
                ON SPLIT(META(stock1).id, ""::"")[1] = META(t1).id
            JOIN closestStores AS store1
                ON META(stock1).id LIKE store1.id || '%'
            WHERE SEARCH(t1,                    /* vector search using image embedding */
              {{
                ""fields"": [""*""],
                ""query"": {{
                  ""match_none"": """"
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
            GROUP BY t1.name, t1.`desc`, t1.image, t1.price, SEARCH_SCORE(t1)
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
            WITH closestStores AS (                     /* CTE to find closet stores to user */
                SELECT x.name, META(x).id AS id
                FROM whatisthis._default.Stores x
                WHERE SEARCH(x, {{
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
            
            SELECT t1.name, t1.`desc`, t1.image, t1.price, t1.rating,
                ARRAY_AGG({{
                    ""storeName"": store1.name,
                    ""quantity"": stock1.numInStock
                }}) AS Stock
            FROM whatisthis._default.Items AS t1
            LEFT JOIN whatisthis._default.Stock AS stock1
                ON SPLIT(META(stock1).id, ""::"")[1] = META(t1).id
            JOIN closestStores AS store1 
                ON META(stock1).id LIKE store1.id || '%'
            WHERE 1==1
                {WhereMinPrice(request)}            /* min price filter (if any) */
                {WhereMaxPrice(request)}            /* max price filter (if any) */
                {WhereMinRating(request)}           /* min rating filter (if any) */
            GROUP BY t1.name, t1.`desc`, t1.image, t1.price, t1.rating
            ORDER BY t1.name
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
        return $" AND t1.rating >= {request.MinRating}";
    }

    private string WhereMaxPrice(BrowseRequest request)
    {
        if (!request.MaxPrice.HasValue)
            return string.Empty;
        return $" AND t1.price <= {request.MaxPrice} ";
    }

    private string WhereMinPrice(BrowseRequest request)
    {
        if (!request.MinPrice.HasValue)
            return string.Empty;
        return $" AND t1.price >= {request.MinPrice} ";
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