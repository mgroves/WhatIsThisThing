namespace WhatIsThisThing.Core.Services;

public class PageOf<T>
{
    public List<T> Collection { get; set; }
    public int TotalPages { get; set; }
}