using Microsoft.AspNetCore.Mvc;
using WhatIsThisThing.Core.Request;
using WhatIsThisThing.Core.Services;

namespace WhatIsThisThing.Server.Controllers;

public class ItemController : Controller
{
    private readonly IIdentifierService _identifierService;
    private readonly IDataLayer _dataLayer;

    public ItemController(IIdentifierService identifierService, IDataLayer dataLayer)
    {
        _identifierService = identifierService;
        _dataLayer = dataLayer;
    }
    
    [HttpPost]
    [Route("/api/identify")]
    public async Task<IActionResult> Identify([FromBody] IdentifyRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Image))
        {
            return BadRequest("Invalid request.");
        }

        var response = await _identifierService.Identify(request);

        return Ok(response);
    }

    [HttpGet]
    [Route("api/catalog")]
    public async Task<IActionResult> Browse([FromQuery] BrowseRequest request)
    {
        var items = await _dataLayer.BrowseCatalog(request);
        return Ok(items);
    }
}