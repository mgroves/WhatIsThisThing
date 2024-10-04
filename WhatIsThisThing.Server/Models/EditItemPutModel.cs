namespace WhatIsThisThing.Server.Models;

public class EditItemPutModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public int Rating { get; set; }
    public string Image { get; set; }
    public decimal Price { get; set; }
}