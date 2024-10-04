using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

public class Base64ImageConverter
{
    public static async Task<string> ConvertToPngAsync(string base64String)
    {
        // Check if the string starts with a PNG base64 header
        if (base64String.StartsWith("data:image/png;base64,", StringComparison.OrdinalIgnoreCase))
        {
            // If it's already a PNG, return the original string as-is
            return base64String;
        }

        // Strip the base64 header if present
        string cleanBase64 = base64String.Contains(",")
            ? base64String.Substring(base64String.IndexOf(",") + 1)
            : base64String;

        // Convert the base64 string to bytes
        byte[] imageBytes = Convert.FromBase64String(cleanBase64);

        // Create a MemoryStream from the byte array
        using (MemoryStream imageStream = new MemoryStream(imageBytes))
        {
            // Load the image asynchronously from the stream
            using (Image image = await Image.LoadAsync(imageStream))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Save the image as PNG
                    await image.SaveAsync(ms, new PngEncoder());

                    // Convert the PNG to base64
                    return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
                }
            }
        }
    }
}