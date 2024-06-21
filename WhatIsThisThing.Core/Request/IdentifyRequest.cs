namespace WhatIsThisThing.Server.Models.Submit;

public class IdentifyRequest
{
    public string Image { get; set; }
    public GeoCoord Location { get; set; }
}