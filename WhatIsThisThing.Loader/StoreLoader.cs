using Couchbase.KeyValue;

namespace WhatIsThisThing.Loader;

internal class StoreLoader
{
    public static async Task Load(ICouchbaseCollection storeCollection)
    {
        var storesAlreadyExist = await storeCollection.ExistsAsync("store003");
        if (storesAlreadyExist.Exists)
            return;

        var stores = new Dictionary<string, dynamic>();

        stores.Add("store001", new
        {
            Name = "Home Depot Canal Winchester",
            Latitude = 39.857713,
            Longitude = -82.831166
        });
        stores.Add("store002", new
        {
            Name = "Commercial Parts and Service",
            Latitude = 39.984273,
            Longitude = -83.139559
        });
        stores.Add("store003", new
        {
            Name = "Ferguson Plumbing Supply",
            Latitude = 40.041899,
            Longitude = -83.127272
        });
        
        foreach (var store in stores)
        {
            await storeCollection.UpsertAsync(store.Key, store.Value);
        }
    }
}
