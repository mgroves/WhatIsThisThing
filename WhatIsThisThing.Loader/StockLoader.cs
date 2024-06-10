using Couchbase.KeyValue;

namespace WhatIsThisThing.Loader;

internal class StockLoader
{
    public static async Task Load(ICouchbaseCollection stockCollection, int maxItems, int maxStores)
    {
        var rand = new Random();
        for (int itemId = 1; itemId <= maxItems; itemId++)
        {
            for (int storeId = 1; storeId <= maxStores; storeId++)
            {
                // random stock number
                var num = rand.Next(0, 75);
                
                // but 1-in-3 chance it's out of stock
                var outOfStockDice = rand.NextInt64(0, 3);
                if (outOfStockDice == 0)
                    num = 0;
                
                var itemIdString = string.Format("{0:000}", itemId);
                var storeIdString = string.Format("{0:000}", storeId);
                await stockCollection.UpsertAsync($"store{storeIdString}::item{itemIdString}", new { numInStock = num });
            }
        }
    }
}