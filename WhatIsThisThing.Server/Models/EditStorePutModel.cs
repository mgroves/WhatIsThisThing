namespace WhatIsThisThing.Server.Models;

public class EditStorePutModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}