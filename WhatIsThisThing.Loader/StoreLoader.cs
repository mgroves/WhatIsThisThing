using Couchbase.KeyValue;

namespace WhatIsThisThing.Loader;

internal class StoreLoader
{
    public static async Task Load(ICouchbaseCollection storeCollection)
    {
        var stores = new Dictionary<string, dynamic>();

        stores.Add("store001", new
        {
            Name = "Home Depot Canal Winchester",
            Geo = new
            {
                Lat = 39.857713,
                Lon = -82.831166
            }
        });
        stores.Add("store002", new
        {
            Name = "Commercial Parts and Service",
            Geo = new
            {
                Lat = 39.984551,
                Lon = -83.140249
            }
        });
        stores.Add("store003", new
        {
            Name = "Ferguson Plumbing Supply",
            Geo = new
            {
                Lat = 40.041899,
                Lon = -83.127272
            }
        });
        stores.Add("store004", new
        {
            Name = "Shultz Supply Cicero IL",
            Geo = new
            {
                Lat = 41.864082,
                Lon = -87.640781
            }
        });
        stores.Add("store005", new
        {
            Name = "DMART Innovations Berlin",
            Geo = new
            {
                Lat = 52.520916,
                Lon = 13.387609
            }
        });
        stores.Add("store006", new
        {
            Name = "Robert Dyas St Martin's Lane",
            Geo = new
            {
                Lat = 51.510596,
                Lon = -0.126951
            }
        });


        foreach (var store in stores)
        {
            await storeCollection.UpsertAsync(store.Key, store.Value);
        }
    }
}
