namespace WhatIsThisThing.Server.Models;

public class NewStockFormModel
{
    public string ItemId { get; set; }
    public string StoreId { get; set; }
    public int NumInStock { get; set; }
}