namespace WhatIsThisThing.Server.Models;

public class IdentifyResponse
{
    public Item IdentifiedItem { get; set; }
    public List<Item> RelatedItems { get; set; }
}

public class StockAvailability
{
    public string StoreName { get; set; }
    public int Quantity { get; set; }
}

public class Item
{
    public string Name { get; set; }
    public string Desc { get; set; }
    public decimal Price { get; set; }
    public string Image { get; set; }
    public List<StockAvailability> Stock { get; set; }
}
