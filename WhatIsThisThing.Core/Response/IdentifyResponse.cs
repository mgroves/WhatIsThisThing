namespace WhatIsThisThing.Server.Models.Response;

public class IdentifyResponse
{
    public ItemResponse IdentifiedItem { get; set; }
    public List<ItemResponse> RelatedItems { get; set; }
}

public class ItemResponse
{
    public string Name { get; set; }
    public string Desc { get; set; }
    public decimal Price { get; set; }
    public string Image { get; set; }
    public List<StockAvailabilityResponse> Stock { get; set; }
}

public class StockAvailabilityResponse
{
    public string StoreName { get; set; }
    public int Quantity { get; set; }
}