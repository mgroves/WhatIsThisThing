using Couchbase;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Management.Collections;
using Couchbase.Search;
using Couchbase.Search.Queries.Vector;
using WhatIsThisThing.Server.Models.Domain;
using WhatIsThisThing.Server.Models.Response;
using WhatIsThisThing.Server.Models.Submit;

namespace WhatIsThisThing.Server.Services;

public interface IDataLayer
{
    Task<List<ItemResponse>> FindItemsWithStockByVectorAndLocation(float[] embedding, Location requestLocation);
    Task<List<Item>> Browse(int page);
}

public class DataLayer : IDataLayer
{
    private const int PAGE_SIZE = 6;
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
            WITH stockCte AS (
	            SELECT SPLIT(META(stock1).id, ""::"")[1] AS itemId, store1.name AS storeName, stock1.numInStock AS quantity
	            FROM whatisthis._default.Stock stock1
	            JOIN whatisthis._default.Stores store1 ON META(stock1).id LIKE META(store1).id || '%'
            )
            SELECT t1.name, t1.`desc`, t1.image, t1.price, s as Stock, SEARCH_SCORE(t1) AS score
            FROM whatisthis._default.Items AS t1
            NEST stockCte s ON s.itemId = META(t1).id
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

    public async Task<List<Item>> Browse(int page)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;

        var sql = @$"
            SELECT META(t1).id, t1.name, t1.`desc`, t1.image, t1.price
            FROM whatisthis._default.Items AS t1
            ORDER BY t1.name
            LIMIT {PAGE_SIZE}
            OFFSET {page * PAGE_SIZE}";

        var result = await cluster.QueryAsync<Item>(sql);
        var rows = result.Rows.AsAsyncEnumerable();
        return await rows.ToListAsync();
    }
}