using Microsoft.AspNetCore.Mvc;
using WhatIsThisThing.Server.Models.Submit;
using WhatIsThisThing.Server.Services;

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