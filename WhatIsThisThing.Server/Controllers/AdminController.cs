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

    #region stores

    [HttpGet]
    [Authorize]
    [Route("/api/admin/stores")]
    public async Task<IActionResult> GetStores(int? page = 0)
    {
        var pageOfItems = await _adminDataLayer.GetAllStores(page);
        return Ok(new { totalPages = pageOfItems.TotalPages, items = pageOfItems.Collection });
    }

    [HttpPost]
    [Route("/api/admin/stores")]
    [Authorize]
    public async Task<IActionResult> AddStore([FromBody] NewStorePostModel newStore)
    {
        await _adminServices.AddStore(
            newStore.Name,
            newStore.Latitude,
            newStore.Longitude);

        return Ok("Store added");
    }

    [HttpDelete]
    [Route("/api/admin/stores/{storeId}")]
    [Authorize]
    public async Task<IActionResult> DeleteStore(string storeId)
    {
        await _adminDataLayer.DeleteStore(storeId);
        return Ok("Store deleted");
    }

    [HttpPut]
    [Route("/api/admin/stores/{storeId}")]
    [Authorize]
    public async Task<IActionResult> EditStore([FromBody] EditStorePutModel editStore, string storeId)
    {
        await _adminServices.EditStore(
            storeId,
            editStore.Name,
            editStore.Latitude,
            editStore.Longitude);

        return Ok("Store updated");
    }

    #endregion


    #region items

    [HttpGet]
    [Authorize]
    [Route("/api/admin/items")]
    public async Task<IActionResult> GetItems(int? page = 0)
    {
        var pageOfItems = await _adminDataLayer.GetAllItems(page);
        return Ok(new { totalPages = pageOfItems.TotalPages, items = pageOfItems.Collection});
    }

    [HttpPost]
    [Route("/api/admin/items")]
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
    [Route("/api/admin/items/{itemId}")]
    [Authorize]
    public async Task<IActionResult> DeleteItem(string itemId)
    {
        await _adminDataLayer.DeleteItem(itemId);
        return Ok("Item deleted");
    }

    [HttpPut]
    [Route("/api/admin/items/{itemId}")]
    [Authorize]
    public async Task<IActionResult> EditItem([FromBody] EditItemPutModel editItem, string itemId)
    {
        await _adminServices.EditItem(
            itemId,
            editItem.Desc,
            editItem.Image,
            editItem.Name,
            editItem.Price,
            editItem.Rating);

        return Ok("Item updated");
    }
    #endregion

}