namespace WhatIsThisThing.Server.Models;

public class NewItemPostModel
{
    public string Name { get; set; }
    public string Desc { get; set; }
    public int Rating { get; set; }
    public string Image { get; set; }
    public decimal Price { get; set; }
}