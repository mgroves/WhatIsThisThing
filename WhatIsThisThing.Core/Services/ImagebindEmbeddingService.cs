using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Polly;

namespace WhatIsThisThing.Core.Services;

public interface IEmbeddingService
{
    Task<float[]> GetImageEmbedding(string requestImage);
}

public class ImagebindEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;

    // https://replicate.com/daanelson
    private static readonly string _version = "0383f62e173dc821ec52663ed22a076d9c970549c209666ac3db181618b7a304";
    private static readonly string _apiKey = "r8_CQ8M7sNDW5Wld7pcADhvBANsFwtw3iF2jZAKX";
    private static readonly string _apiEndpoint = "https://api.replicate.com/v1/predictions";

    public ImagebindEmbeddingService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<float[]> GetImageEmbedding(string base64EncodedImage)
    {
        var data = new
        {
            version = _version,
            input = new
            {
                input = base64EncodedImage,
                modality = "vision"
            }
        };

        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_apiEndpoint, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseString);
        var getUrl = responseObject.Urls.Get;

        return await GetEmbeddingOutputAsync(_httpClient, getUrl);
    }

    private static async Task<float[]> GetEmbeddingOutputAsync(HttpClient client, string url)
    {
        var retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(response =>
            {
                var getResponseString = response.Content.ReadAsStringAsync().Result;
                var getResponseObject = JsonConvert.DeserializeObject<GetApiResponse>(getResponseString);
                return getResponseObject.Status != "succeeded";
            })
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, context) =>
                {
                    // Optional: Add logic to handle each retry attempt if needed
                });

        var getResponse = await retryPolicy.ExecuteAsync(() => client.GetAsync(url));
        var getResponseString = await getResponse.Content.ReadAsStringAsync();
        var getResponseObject = JsonConvert.DeserializeObject<GetApiResponse>(getResponseString);

        if (getResponseObject.Status == "succeeded")
            return getResponseObject.Output;
        
        // Handle the case where the status is not "succeeded" after all retries
        throw new Exception("Vector Embedding retrieval operation did not succeed after 5 retries.");
    }

    public class GetApiResponse
    {
        public float[] Output { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
    }

    public class ApiResponse
    {
        public UrlsData Urls { get; set; }
    }

    public class UrlsData
    {
        public string Get { get; set; }
    }
}

