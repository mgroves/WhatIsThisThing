namespace WhatIsThisThing.Server.Models.Submit;

public class BrowseRequest
{
    public int Page { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class LocationsRequest
{
    public int Page { get; set; }
}