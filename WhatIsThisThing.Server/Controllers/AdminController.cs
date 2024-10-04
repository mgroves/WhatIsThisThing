using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatIsThisThing.Core.Services;
using WhatIsThisThing.Server.Models;

namespace WhatIsThisThing.Server.Controllers;

public class AdminController : Controller
{
    private readonly AdminDataLayer _adminDataLayer;
    private readonly AdminServices _adminServices;

    public AdminController(AdminDataLayer adminDataLayer, AdminServices adminServices)
    {
        _adminDataLayer = adminDataLayer;
        _adminServices = adminServices;
    }
    
    [HttpGet]
    [Authorize]
    [Route("/api/items")]
    public async Task<IActionResult> GetItems(int? page = 0)
    {
        var pageOfItems = await _adminDataLayer.GetAllItems(page);
        return Ok(new { totalPages = pageOfItems.TotalPages, items = pageOfItems.Collection});
    }

    [HttpPost]
    [Route("/api/items")]
    [Authorize]
    public async Task<IActionResult> AddItem([FromBody] NewItemPostModel newItem)
    {
        await _adminServices.AddItem(
            newItem.Desc,
            newItem.Image,
            newItem.Name,
            newItem.Price,
            newItem.Rating);

        return Ok("Item added");
    }

    [HttpDelete]
    [Route("/api/items/{itemId}")]
    [Authorize]
    public async Task<IActionResult> DeleteItem(string itemId)
    {
        await _adminDataLayer.DeleteItem(itemId);
        return Ok("Item deleted");
    }

    [HttpPut]
    [Route("/api/items/{itemId}")]
    [Authorize]
    public async Task<IActionResult> EditItem([FromBody] EditItemPutModel editItem)
    {
        await _adminServices.EditItem(
            editItem.Id,
            editItem.Desc,
            editItem.Image,
            editItem.Name,
            editItem.Price,
            editItem.Rating);

        return Ok("Item added");
    }
    
}