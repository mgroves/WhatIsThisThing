using SixLabors.ImageSharp;

namespace WhatIsThisThing.Loader;

public class ImageHelper
{
    public static async Task<string> ImageToBase64(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            throw new ArgumentException("Image path cannot be null or empty", nameof(imagePath));
        }

        try
        {
            using (Image image = await Image.LoadAsync(imagePath))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await image.SaveAsPngAsync(memoryStream); // standardize to PNG format
                byte[] imageBytes = memoryStream.ToArray();
                return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions as needed
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}