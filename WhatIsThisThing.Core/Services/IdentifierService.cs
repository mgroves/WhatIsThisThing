using WhatIsThisThing.Core.Services;
using WhatIsThisThing.Server.Models.Domain;
using WhatIsThisThing.Server.Models.Response;
using WhatIsThisThing.Server.Models.Submit;

namespace WhatIsThisThing.Server.Services;

public interface IIdentifierService
{
    Task<IdentifyResponse> Identify(IdentifyRequest request);
}

public class IdentifierService : IIdentifierService
{
    private readonly IEmbeddingService _embedding;
    private readonly IDataLayer _data;

    public IdentifierService(IEmbeddingService embedding, IDataLayer data)
    {
        _embedding = embedding;
        _data = data;
    }
    
    public async Task<IdentifyResponse> Identify(IdentifyRequest request)
    {
        // get embedding
        var embedding = await _embedding.GetImageEmbedding(request.Image);
        
        // search database
        var items = await _data.FindItemsByVector(embedding, request.Location);

        // find locations
        var stores = await _data.FindNearbyStores(request.Location);

        var bestItem = items.First();
        var relatedItems = items.Skip(1);

        var result = new IdentifyResponse();

        result.IdentifiedItem = await GetStock(bestItem, stores);
        result.RelatedItems = (await Task.WhenAll(relatedItems.Select(i => GetStock(i, stores)))).ToList();
        
        return result;
    }

    private async Task<ItemResponse> GetStock(Item bestItem, List<Store> stores)
    {
        return new ItemResponse
        {
            Desc = bestItem.Desc,
            Image = bestItem.Image,
            Name = bestItem.Name,
            Price = bestItem.Price,
            Stock = new List<StockAvailabilityResponse>
            {
                new StockAvailabilityResponse { Quantity = 5, StoreName = stores[0].Name },
                new StockAvailabilityResponse { Quantity = 0, StoreName = stores[1].Name },
                new StockAvailabilityResponse { Quantity = 1, StoreName = stores[2].Name }
            }
        };
    }
}