using Couchbase.Extensions.DependencyInjection;
using Couchbase.Management.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WhatIsThisThing.Core;
using WhatIsThisThing.Core.Services;
using WhatIsThisThing.Loader;

Console.WriteLine("Initializing...");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddConfiguration(ConfigurationHelper.GetConfiguration());
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddCouchbase(configuration.GetSection("Couchbase"));
        services.AddTransient<IEmbeddingService, AzureEmbeddingService>();
    })
.Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var bucketName = configuration.GetValue<string>("Couchbase:BucketName");
var scopeName = configuration.GetValue<string>("Couchbase:ScopeName");
var bucketProvider = host.Services.GetService<IBucketProvider>();
var bucket = await bucketProvider.GetBucketAsync(bucketName);
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
try
{
    var defaultCollectionSettings = new CreateCollectionSettings();
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

IEmbeddingService embed = host.Services.GetService<IEmbeddingService>();

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
