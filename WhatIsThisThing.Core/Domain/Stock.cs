namespace WhatIsThisThing.Core.Domain;

public class Stock
{
    public string Id { get; set; }

    public string ItemId => Id.Split("::")[1];

    public string StoreId => Id.Split("::")[0];

    public int NumInStock { get; set; }
}