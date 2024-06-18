using WhatIsThisThing.Core.Services;
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
        var items = await _data.FindItemsWithStockByVectorAndLocation(embedding, request.Location);

        var result = new IdentifyResponse();

        result.IdentifiedItem = items.First();
        result.RelatedItems = items.Skip(1).ToList();
        
        return result;
    }

}