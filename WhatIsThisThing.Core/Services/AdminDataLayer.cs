using Couchbase.Extensions.DependencyInjection;
using WhatIsThisThing.Core.Domain;
using Couchbase.KeyValue;

namespace WhatIsThisThing.Core.Services;

public class AdminDataLayer
{
    private const ulong INITIAL_CUSTOM_ITEM_ID = 300;
    private readonly IBucketProvider _bucketProvider;

    public AdminDataLayer(IBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task<List<Item>> GetAllItems()
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;
        var sql = "SELECT META(i).id, i.name, i.`desc`, i.price, i.rating, i.image FROM whatisthis._default.Items i ORDER BY i.name ASC;";
        var query = await cluster.QueryAsync<Item>(sql);
        return await query.Rows.ToListAsync();
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