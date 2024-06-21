using Microsoft.AspNetCore.Mvc;
using WhatIsThisThing.Core.Request;
using WhatIsThisThing.Core.Services;

namespace WhatIsThisThing.Server.Controllers;

public class StoreController : Controller
{
    private readonly IDataLayer _dataLayer;

    public StoreController(IDataLayer dataLayer)
    {
        _dataLayer = dataLayer;
    }

    [HttpGet]
    [Route("api/stores")]
    public async Task<IActionResult> Stores([FromQuery] LocationsRequest request)
    {
        var items = await _dataLayer.GetStores(request);
        return Ok(new { data = items });
    }
}