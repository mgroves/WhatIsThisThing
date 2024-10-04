namespace WhatIsThisThing.Core.Domain;

public class Item
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public decimal Price { get; set; }
    public string Image { get; set; }
    public int Rating { get; set; }
    public float[] ImageVector { get; set; }
}
