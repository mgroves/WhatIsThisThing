using Couchbase.Extensions.DependencyInjection;
using WhatIsThisThing.Core.Domain;
using Couchbase.KeyValue;
using Couchbase.Query;
using Newtonsoft.Json.Linq;

namespace WhatIsThisThing.Core.Services;

public class AdminDataLayer
{
    private const int PAGE_SIZE = 10;
    private const ulong INITIAL_CUSTOM_ITEM_ID = 300;
    private readonly IBucketProvider _bucketProvider;

    public AdminDataLayer(IBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task<PageOf<Item>> GetAllItems(int? pageNumber = 0)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;
        var sql = $@"WITH total AS (SELECT COUNT(1) AS totalItems FROM whatisthis._default.Items)
                    SELECT META(i).id, i.name, i.`desc`, i.price, i.rating, i.image,
                        CEIL(total.totalItems / $pageSize) AS totalPages
                    FROM whatisthis._default.Items i, total
                    ORDER BY i.name ASC
                    LIMIT $pageSize
                    OFFSET $offset;";
        var result = await cluster.QueryAsync<JObject>(sql, new QueryOptions()
            .ScanConsistency(QueryScanConsistency.RequestPlus)
            .Parameter("pageSize", PAGE_SIZE)
            .Parameter("offset", (pageNumber ?? 0) * PAGE_SIZE)
        );
        var items = await result.Rows.ToListAsync();
        var totalPages = (int)(items.FirstOrDefault()?["totalPages"] ?? 0);

        // super hacky, but this is how I can grab TotalCount AND the inventories in a single query
        var page = new PageOf<Item>();
        page.Collection = items.Select(i => i.ToObject<Item>()).ToList();
        page.TotalPages = totalPages;

        return page;
    }

    public async Task AddItem(Item item)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Items");
        var counterColl = await bucket.DefaultCollectionAsync();
        
        // get increment ID from counter document (create if doesn't exist)
        var incr = await counterColl.Binary.IncrementAsync("itemCounter", options =>
        {
            options.Initial(INITIAL_CUSTOM_ITEM_ID);
            options.Delta(1);
        });
        
        await coll.InsertAsync($"item{incr.Content}", new
        {
            item.Image,
            item.Desc,
            item.ImageVector,
            item.Name,
            item.Price,
            item.Rating
        });
    }

    public async Task DeleteItem(string itemId)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Items");

        await coll.RemoveAsync(itemId);
        
        // TODO: also delete stock for this item?
    }

    public async Task UpdateItem(Item item)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Items");

        await coll.ReplaceAsync(item.Id, new
        {
            item.Image,
            item.Desc,
            item.ImageVector,
            item.Name,
            item.Price,
            item.Rating
        });
    }

    public async Task<Item> GetItemById(string itemId)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Items");
        var itemDoc = await coll.GetAsync(itemId);
        var item = itemDoc.ContentAs<Item>();
        item.Id = itemId;
        return item;
    }
}