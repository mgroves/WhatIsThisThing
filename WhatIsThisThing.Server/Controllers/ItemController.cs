using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatIsThisThing.Server.Models.Response;
using WhatIsThisThing.Server.Models.Submit;
using WhatIsThisThing.Server.Services;

namespace WhatIsThisThing.Server.Controllers;

public class ItemController : Controller
{
    private readonly IIdentifierService _identifierService;

    public ItemController(IIdentifierService identifierService)
    {
        _identifierService = identifierService;
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

        return Ok(new { data = response });
    }
}