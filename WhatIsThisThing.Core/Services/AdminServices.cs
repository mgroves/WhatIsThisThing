using System.ComponentModel.DataAnnotations;
using WhatIsThisThing.Core.Domain;

namespace WhatIsThisThing.Core.Services;

public class AdminServices
{
    private readonly IEmbeddingService _embeddingService;
    private readonly AdminDataLayer _adminDataLayer;

    public AdminServices(IEmbeddingService embeddingService, AdminDataLayer adminDataLayer)
    {
        _embeddingService = embeddingService;
        _adminDataLayer = adminDataLayer;
    }
    
    public async Task AddItem(string newItemDesc, string newItemImage, string newItemName, decimal newItemPrice, int newItemRating)
    {
        // validate
        if (string.IsNullOrEmpty(newItemDesc))
            throw new ValidationException("Description is required");
        if (string.IsNullOrEmpty(newItemImage))
            throw new ValidationException("Image is required");
        if (string.IsNullOrEmpty(newItemName))
            throw new ValidationException("Name is required");
        if (newItemPrice <= 0)
            throw new ValidationException("Positive price is required");
        if (newItemRating <= 0 || newItemRating > 5)
            throw new ValidationException("Rating between 1 to 5 is required");

        // convert image to PNG
        newItemImage = await Base64ImageConverter.ConvertToPngAsync(newItemImage);
        
        // get embedding
        var embedding = await _embeddingService.GetImageEmbedding(newItemImage);

        // add to database
        await _adminDataLayer.AddItem(new Item
        {
            Desc = newItemDesc,
            Image = newItemImage,
            Name = newItemName,
            Price = newItemPrice,
            ImageVector = embedding,
            Rating = newItemRating
        });
    }

    public async Task EditItem(string editItemId, string editItemDesc, string editItemImage, string editItemName, decimal editItemPrice, int editItemRating)
    {
        // validate
        if (string.IsNullOrEmpty(editItemId))
            throw new ValidationException("Error: ID not found");
        if (string.IsNullOrEmpty(editItemDesc))
            throw new ValidationException("Description is required");
        if (string.IsNullOrEmpty(editItemImage))
            throw new ValidationException("Image is required");
        if (string.IsNullOrEmpty(editItemName))
            throw new ValidationException("Name is required");
        if (editItemPrice <= 0)
            throw new ValidationException("Positive price is required");
        if (editItemRating <= 0 || editItemRating > 5)
            throw new ValidationException("Rating between 1 to 5 is required");

        // convert image to PNG
        editItemImage = await Base64ImageConverter.ConvertToPngAsync(editItemImage);

        // if image hasn't changed, no need to get a new embedding
        var existing = await _adminDataLayer.GetItemById(editItemId);
        float[] embedding;
        if (existing.Image != editItemImage)
            embedding = await _embeddingService.GetImageEmbedding(editItemImage);
        else
            embedding = existing.ImageVector;

        await _adminDataLayer.UpdateItem(new Item
        {
            Id = editItemId,
            Desc = editItemDesc,
            Image = editItemImage,
            Name = editItemName,
            Price = editItemPrice,
            ImageVector = embedding,
            Rating = editItemRating
        });
    }

    public async Task AddStore(string newStoreName, float newStoreLatitude, float newStoreLongitude)
    {
        // validate
        if (string.IsNullOrEmpty(newStoreName))
            throw new ValidationException("Name is required");
        if (!(newStoreLatitude >= -90 && newStoreLatitude <= 90))
            throw new ValidationException("Latitude is not valid");
        if (!(newStoreLongitude >= -180 && newStoreLongitude <= 180))
            throw new ValidationException("Longitude is not valid");

        await _adminDataLayer.AddStore(new Store
        {
            Latitude = newStoreLatitude,
            Longitude = newStoreLongitude,
            Name = newStoreName
        });
    }

    public async Task EditStore(string editStoreId, string editStoreName, float editStoreLatitude, float editStoreLongitude)
    {
        if (string.IsNullOrEmpty(editStoreId))
            throw new ValidationException("Error: ID is required");
        if (string.IsNullOrEmpty(editStoreName))
            throw new ValidationException("Name is required");
        if (!(editStoreLatitude >= -90 && editStoreLatitude <= 90))
            throw new ValidationException("Latitude is not valid");
        if (!(editStoreLongitude >= -180 && editStoreLongitude <= 180))
            throw new ValidationException("Longitude is not valid");

        await _adminDataLayer.UpdateStore(new Store
        {
            Id = editStoreId,
            Latitude = editStoreLatitude,
            Longitude = editStoreLongitude,
            Name = editStoreName
        });
    }
}