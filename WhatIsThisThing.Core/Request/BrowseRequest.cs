﻿namespace WhatIsThisThing.Core.Request;

public class BrowseRequest
{
    public int Page { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}