using Couchbase;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Management.Collections;
using Couchbase.Search;
using Couchbase.Search.Queries.Vector;
using WhatIsThisThing.Server.Models.Domain;
using WhatIsThisThing.Server.Models.Submit;

namespace WhatIsThisThing.Server.Services;

public interface IDataLayer
{
    Task<List<Item>> FindItemsByVector(float[] embedding, Location requestLocation);
    Task<List<Store>> FindNearbyStores(Location requestLocation);
}

public class DataLayer : IDataLayer
{
    private readonly IBucketProvider _bucketProvider;

    public DataLayer(IBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }
    
    public async Task<List<Item>> FindItemsByVector(float[] embedding, Location requestLocation)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;

        var sql = @$"
            SELECT META(t1).id, t1.name, t1.`desc`, t1.image, t1.price, SEARCH_SCORE() AS score
            FROM whatisthis._default.Items AS t1
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


        var result = await cluster.QueryAsync<Item>(sql);
        var rows = result.Rows.AsAsyncEnumerable();
        return await rows.ToListAsync();
    }

    public async Task<List<Store>> FindNearbyStores(Location requestLocation)
    {
        var list = new List<Store>
        {
            new Store { Name = "Home Depot Canal Winchester", Latitude = 39.857713, Longitude = -82.831166},
            new Store { Name = "Commercial Parts and Service", Latitude = 39.984273, Longitude = -83.139559},
            new Store { Name = "Ferguson Plumbing Supply", Latitude = 40.041899, Longitude = -83.127272}
        };
        return list;
    }
}