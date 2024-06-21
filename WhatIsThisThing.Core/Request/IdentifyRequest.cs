using WhatIsThisThing.Core.Domain;

namespace WhatIsThisThing.Core.Request;

public class IdentifyRequest
{
    public string Image { get; set; }
    public GeoCoord Location { get; set; }
}