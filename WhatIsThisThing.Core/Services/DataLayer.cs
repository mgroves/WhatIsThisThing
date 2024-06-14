using Couchbase.Extensions.DependencyInjection;
using WhatIsThisThing.Server.Models.Domain;
using WhatIsThisThing.Server.Models.Response;
using WhatIsThisThing.Server.Models.Submit;

namespace WhatIsThisThing.Server.Services;

public interface IDataLayer
{
    Task<List<ItemResponse>> FindItemsWithStockByVectorAndLocation(float[] embedding, Location requestLocation);
    Task<List<ItemResponse>> Browse(BrowseRequest request);
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
    
    public async Task<List<ItemResponse>> FindItemsWithStockByVectorAndLocation(float[] embedding, Location requestLocation)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;

        var sql = @$"
            WITH closestStores AS (
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
            ),
            stockCte AS (
	            SELECT SPLIT(META(stock1).id, ""::"")[1] AS itemId, store1.name AS storeName, stock1.numInStock AS quantity
	            FROM whatisthis._default.Stock stock1
	            JOIN closestStores store1 ON META(stock1).id LIKE store1.id || '%'
            )
            SELECT t1.name, t1.`desc`, t1.image, t1.price, s as Stock, SEARCH_SCORE(t1) AS score
            FROM whatisthis._default.Items AS t1
            LEFT NEST stockCte s ON s.itemId = META(t1).id
            WHERE SEARCH(t1,
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
            ORDER BY score DESC";


        var result = await cluster.QueryAsync<ItemResponse>(sql);
        var rows = result.Rows.AsAsyncEnumerable();
        return await rows.ToListAsync();
    }

    public async Task<List<ItemResponse>> Browse(BrowseRequest request)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;

        var sql = @$"
            WITH closestStores AS (
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
            ),
            stockCte AS (
	            SELECT SPLIT(META(stock1).id, ""::"")[1] AS itemId, store1.name AS storeName, stock1.numInStock AS quantity
	            FROM whatisthis._default.Stock stock1
	            JOIN closestStores store1 ON META(stock1).id LIKE store1.id || '%'
            )
            SELECT t1.name, t1.`desc`, t1.image, t1.price, s as Stock, SEARCH_SCORE(t1) AS score
            FROM whatisthis._default.Items AS t1
            LEFT NEST stockCte s ON s.itemId = META(t1).id
            ORDER BY t1.name
            LIMIT {PAGE_SIZE}
            OFFSET {request.Page * PAGE_SIZE}";

        var result = await cluster.QueryAsync<ItemResponse>(sql);
        var rows = result.Rows.AsAsyncEnumerable();
        return await rows.ToListAsync();
    }
}