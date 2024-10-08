﻿using WhatIsThisThing.Core.Request;
using WhatIsThisThing.Core.Response;

namespace WhatIsThisThing.Core.Services;

public interface IIdentifierService
{
    Task<WithModalInfo<IdentifyResponse>> Identify(IdentifyRequest request);
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
    
    public async Task<WithModalInfo<IdentifyResponse>> Identify(IdentifyRequest request)
    {
        // get embedding
        var embedding = await _embedding.GetImageEmbedding(request.Image);
        
        // search database
        var items = await _data.FindItemsWithStockByVectorAndLocation(embedding, request.Location);

        var result = new WithModalInfo<IdentifyResponse>();

        result.Data = new IdentifyResponse();
        result.Data.IdentifiedItem = items.Data.First();
        result.Data.RelatedItems = items.Data.Skip(1).ToList();
        result.ModalTitle = items.ModalTitle;
        result.ModalContent = items.ModalContent;
        
        return result;
    }

}