using Couchbase;
using Couchbase.Management.Collections;
using WhatIsThisThing.Core.Services;
using WhatIsThisThing.Loader;

Console.WriteLine("Initializing...");

var cluster = await Cluster.ConnectAsync("couchbase://localhost", options =>
{
    options.UserName = "Administrator";
    options.Password = "password";
});

var bucketName = "whatisthis";
var scopeName = "_default";

var bucket = await cluster.BucketAsync(bucketName);
var collectionManager = bucket.Collections;

Console.WriteLine("Done initializing.");

Console.WriteLine("Creating Couchbase scope (if necessary)...");

if (scopeName != "_default")
{
    try
    {
        await collectionManager.CreateScopeAsync(scopeName);
    }
    catch (ScopeExistsException)
    {
        // it's fine if scope already exists
    }
}

Console.WriteLine("Done creating Couchbase scope.");

Console.WriteLine("Creating Couchbase collections...");

// create collections
var defaultCollectionSettings = new CreateCollectionSettings();
try
{
    await collectionManager.CreateCollectionAsync(scopeName, "Items", defaultCollectionSettings);
    await collectionManager.CreateCollectionAsync(scopeName, "Stock", defaultCollectionSettings);
    await collectionManager.CreateCollectionAsync(scopeName, "Stores", defaultCollectionSettings);
}
catch (CollectionExistsException)
{
    // it's fine if collections already exist
}
Console.WriteLine("Done creating Couchbase collections...");

// ******** load initial demo data

IEmbeddingService embed = new AzureEmbeddingService();

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

// load stock
Console.WriteLine("Loading demo stock...");
var stockCollection = await bucket.CollectionAsync("Stock");
await StockLoader.Load(stockCollection, maxItems: 15, maxStores: 6);
Console.WriteLine("Done loading demo stock.");

// TODO: create vector index?
