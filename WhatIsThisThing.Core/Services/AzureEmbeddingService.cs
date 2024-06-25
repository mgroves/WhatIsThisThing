using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace WhatIsThisThing.Core.Services;

public class AzureEmbeddingService : IEmbeddingService
{
    private readonly IOptions<AzureComputerVisionSettings> _settings;

    public AzureEmbeddingService(IOptions<AzureComputerVisionSettings> settings)
    {
        _settings = settings;
    }
    
    // Free tier: 20 Calls per minute, 5K Calls per month
    // Standard tier: 10 Calls per second, starting $1.00 USD/1000 calls (Estimated)
    public async Task<float[]> GetImageEmbedding(string base64Image)
    {
        var endpoint = _settings.Value.Endpoint;
        var subscriptionKey = _settings.Value.SubscriptionKey;

        using (HttpClient client = new HttpClient())
        {
            // Set the subscription key and endpoint
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Endpoint URL
            string url = $"{endpoint}/retrieval:vectorizeImage?overload=stream&api-version=2023-04-01-preview";

            byte[] imageBytes = Base64PngToByteArray(base64Image);

            using (ByteArrayContent content = new ByteArrayContent(imageBytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                HttpResponseMessage response = await client.PostAsync(url, content);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON response to extract the vector embeddings
                    JObject json = JObject.Parse(jsonResponse);
                    JToken vectorEmbeddings = json["vector"];
                    return vectorEmbeddings.ToObject<float[]>();
                }

                throw new Exception("Unable to retrieve vector embeddings for image.");
            }
        }
    }

    private byte[] Base64PngToByteArray(string base64Png)
    {
        if (string.IsNullOrEmpty(base64Png))
        {
            throw new ArgumentException("Base64 PNG string cannot be null or empty", nameof(base64Png));
        }

        try
        {
            // Remove the data:image/png;base64, prefix if present
            string base64String = base64Png.StartsWith("data:image/png;base64,")
                ? base64Png.Substring("data:image/png;base64,".Length)
                : base64Png;

            return Convert.FromBase64String(base64String);
        }
        catch (Exception ex)
        {
            // Handle exceptions as needed
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }
    }
}