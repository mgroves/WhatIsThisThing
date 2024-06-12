﻿using Couchbase;
using Couchbase.Management.Collections;
using WhatIsThisThing.Core.Services;
using WhatIsThisThing.Loader;

Console.WriteLine("Initializing...");

var embed = new AzureEmbeddingService();

var cluster = await Cluster.ConnectAsync("couchbase://localhost", options =>
{
    options.UserName = "Administrator";
    options.Password = "password";
});

var bucketName = "whatisthis";

var bucket = await cluster.BucketAsync(bucketName);
var collectionManager = bucket.Collections;

Console.WriteLine("Done initializing.");

Console.WriteLine("Creating Couchbase collections...");

// create collections
var defaultCollectionSettings = new CreateCollectionSettings();
try
{
    await collectionManager.CreateCollectionAsync("_default", "Items", defaultCollectionSettings);
    await collectionManager.CreateCollectionAsync("_default", "Stock", defaultCollectionSettings);
    await collectionManager.CreateCollectionAsync("_default", "Stores", defaultCollectionSettings);
}
catch (CollectionExistsException)
{
    // it's fine if collections already exist
}
Console.WriteLine("Done creating Couchbase collections...");

// TODO: load initial demo data

// load items if necessary
Console.WriteLine("Loading demo items...");
var itemCollection = await bucket.CollectionAsync("Items");
var itemLoader = new ItemLoader(itemCollection, embed);
await itemLoader.Load();
Console.WriteLine("Done loading demo items.");

// load stores
Console.WriteLine("Loading demo stores...");
var storeCollection = await bucket.CollectionAsync("Stores");
await StoreLoader.Load(storeCollection);
Console.WriteLine("Done loading demo stores.");

// TODO: load stock
Console.WriteLine("Loading demo stock...");
var stockCollection = await bucket.CollectionAsync("Stock");
await StockLoader.Load(stockCollection, maxItems: 15, maxStores: 3);
Console.WriteLine("Done loading demo stock.");

// TODO: create vector index?
