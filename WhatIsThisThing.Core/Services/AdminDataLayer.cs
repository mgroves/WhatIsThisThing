﻿using Couchbase.Extensions.DependencyInjection;
using WhatIsThisThing.Core.Domain;
using Couchbase.KeyValue;
using Couchbase.Query;
using Newtonsoft.Json.Linq;
using Couchbase;

namespace WhatIsThisThing.Core.Services;

public class AdminDataLayer
{
    private const int PAGE_SIZE = 10;
    private const ulong INITIAL_CUSTOM_ITEM_ID = 300;
    private const ulong INITIAL_STORE_ITEM_ID = 300;
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
        
        // delete the item
        await coll.RemoveAsync(itemId);
        
        // delete all stock for this item
        var cluster = bucket.Cluster;
        await cluster.QueryAsync<dynamic>( 
            "DELETE FROM whatisthis._default.Stock WHERE META().id LIKE '%::' || $itemId", new QueryOptions()
                .Parameter("itemId", itemId)
                .ScanConsistency(QueryScanConsistency.RequestPlus)
            );
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

    public async Task<PageOf<Store>> GetAllStores(int? pageNumber = 0)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;
        var sql = $@"WITH total AS (SELECT COUNT(1) AS totalItems FROM whatisthis._default.Stores)
                    SELECT META(s).id, s.name, s.geo.lat as latitude, s.geo.lon as longitude,
                        CEIL(total.totalItems / $pageSize) AS totalPages
                    FROM whatisthis._default.Stores s, total
                    ORDER BY s.name ASC
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
        var page = new PageOf<Store>();
        page.Collection = items.Select(i => i.ToObject<Store>()).ToList();
        page.TotalPages = totalPages;

        return page;
    }

    public async Task AddStore(Store store)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Stores");
        var counterColl = await bucket.DefaultCollectionAsync();

        // get increment ID from counter document (create if doesn't exist)
        var incr = await counterColl.Binary.IncrementAsync("storeCounter", options =>
        {
            options.Initial(INITIAL_STORE_ITEM_ID);
            options.Delta(1);
        });

        await coll.InsertAsync($"store{incr.Content}", new
        {
            store.Name,
            geo = new
            {
                lat = store.Latitude,
                lon = store.Longitude
            }
        });
    }

    public async Task DeleteStore(string storeId)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Stores");
        await coll.RemoveAsync(storeId);

        // also delete stock for this store
        var cluster = bucket.Cluster;
        await cluster.QueryAsync<dynamic>(
            "DELETE FROM whatisthis._default.Stock WHERE META().id LIKE $storeId || '::%'", new QueryOptions()
                .Parameter("storeId", storeId)
                .ScanConsistency(QueryScanConsistency.RequestPlus)
        );
    }

    public async Task UpdateStore(Store store)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Stores");

        await coll.ReplaceAsync(store.Id, new
        {
            name = store.Name,
            geo = new
            {
                lat = store.Latitude,
                lon = store.Longitude
            }
        });
    }

    public async Task<PageOf<Stock>> GetAllStock(int? pageNumber)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;
        var sql = $@"WITH total AS (SELECT COUNT(1) AS totalItems FROM whatisthis._default.Stock)
                    SELECT META(s).id, s.numInStock,
                        CEIL(total.totalItems / $pageSize) AS totalPages
                    FROM whatisthis._default.Stock s, total
                    ORDER BY META(s).id ASC
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
        var page = new PageOf<Stock>();
        page.Collection = items.Select(i => i.ToObject<Stock>()).ToList();
        page.TotalPages = totalPages;

        return page;
    }

    public async Task<List<Store>> GetAllStoresIdAndNameOnly()
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;
        var sql = $@"SELECT META(s).id, s.name
                    FROM whatisthis._default.Stores s
                    ORDER BY s.name ASC";
        var result = await cluster.QueryAsync<Store>(sql, new QueryOptions()
            .ScanConsistency(QueryScanConsistency.NotBounded)
        );
        return await result.Rows.ToListAsync();
    }

    public async Task<List<Item>> GetAllItemsIdAndNameOnly()
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var cluster = bucket.Cluster;
        var sql = $@"SELECT META(i).id, i.name
                    FROM whatisthis._default.Items i
                    ORDER BY i.name ASC";
        var result = await cluster.QueryAsync<Item>(sql, new QueryOptions()
            .ScanConsistency(QueryScanConsistency.NotBounded)
        );
        return await result.Rows.ToListAsync();
    }

    public async Task AddOrUpdateStock(string itemId, string storeId, int numInStock)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Stock");
        await coll.UpsertAsync($"{storeId}::{itemId}", new
        {
            numInStock
        });
    }

    public async Task DeleteStock(string stockKey)
    {
        var bucket = await _bucketProvider.GetBucketAsync("whatisthis");
        var coll = await bucket.CollectionAsync("Stock");
        await coll.RemoveAsync(stockKey);
    }
}