using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatIsThisThing.Server.Models;

namespace WhatIsThisThing.Server.Controllers;

public class ItemController : Controller
{

    [HttpPost]
    [Route("/identify")]
    public async Task<IActionResult> Identify()
    {
        string json = @"{
            ""identifiedItem"": {
                ""name"": ""Mysterious Device"",
                ""desc"": ""The device that's just what you're looking for"",
                ""price"": 199.99,
                ""image"": ""https://picsum.photos/seed/mysteriousitem/300"",
                ""stock"": [
                    { ""storeName"": ""Lancaster"", ""quantity"": 5 },
                    { ""storeName"": ""Grove City"", ""quantity"": 0 },
                    { ""storeName"": ""Canal Winchester"", ""quantity"": 3 }
                ]
            },
            ""relatedItems"": [
                {
                    ""name"": ""Related Device 1"",
                    ""desc"": ""Not quite the same thing but might be of interest"",
                    ""price"": 149.99,
                    ""image"": ""https://picsum.photos/seed/relateditem1/300"",
                    ""stock"": [
                        { ""storeName"": ""Lancaster"", ""quantity"": 2 },
                        { ""storeName"": ""Grove City"", ""quantity"": 1 },
                        { ""storeName"": ""Canal Winchester"", ""quantity"": 0 }
                    ]
                },
                {
                    ""name"": ""Related Device 2"",
                    ""desc"": ""Widget ipsum dolor doohickey"",
                    ""price"": 129.99,
                    ""image"": ""https://picsum.photos/seed/relateditem2/300"",
                    ""stock"": [
                        { ""storeName"": ""Lancaster"", ""quantity"": 4 },
                        { ""storeName"": ""Grove City"", ""quantity"": 0 },
                        { ""storeName"": ""Canal Winchester"", ""quantity"": 2 }
                    ]
                },
                {
                    ""name"": ""Related Device 3"",
                    ""desc"": ""Oh yeah I need one of these too"",
                    ""price"": 179.99,
                    ""image"": ""https://picsum.photos/seed/relateditem3/300"",
                    ""stock"": [
                        { ""storeName"": ""Lancaster"", ""quantity"": 0 },
                        { ""storeName"": ""Grove City"", ""quantity"": 3 },
                        { ""storeName"": ""Canal Winchester"", ""quantity"": 5 }
                    ]
                }
            ]
        }";

        return Ok(new { data = JsonConvert.DeserializeObject<IdentifyResponse>(json)} );

        //return Ok("{\r\n  \"data\": {\r\n    \"identifiedItem\": {\r\n      \"name\": \"Mysterious Device\",\r\n      \"desc\": \"The device that's just what you're looking for\",\r\n      \"price\": 199.99,\r\n      \"image\": \"https://picsum.photos/seed/mysteriousitem/300\",\r\n      \"stores\": [\r\n        { \"name\": \"Lancaster\", \"stock\": 5 },\r\n        { \"name\": \"Grove City\", \"stock\": 0 },\r\n        { \"name\": \"Canal Winchester\", \"stock\": 3 }\r\n      ]\r\n    },\r\n    \"relatedItems\": [\r\n      {\r\n        \"name\": \"Related Device 1\",\r\n        \"desc\": \"Not quite the same thing but might be of interest\",\r\n        \"price\": 149.99,\r\n        \"image\": \"https://picsum.photos/seed/relateditem1/300\",\r\n        \"stores\": [\r\n          { \"name\": \"Lancaster\", \"stock\": 2 },\r\n          { \"name\": \"Grove City\", \"stock\": 1 },\r\n          { \"name\": \"Canal Winchester\", \"stock\": 0 }\r\n        ]\r\n      },\r\n      {\r\n        \"name\": \"Related Device 2\",\r\n        \"desc\": \"Widget ipsum dolor doohickey\",\r\n        \"price\": 129.99,\r\n        \"image\": \"https://picsum.photos/seed/relateditem2/300\",\r\n        \"stores\": [\r\n          { \"name\": \"Lancaster\", \"stock\": 4 },\r\n          { \"name\": \"Grove City\", \"stock\": 0 },\r\n          { \"name\": \"Canal Winchester\", \"stock\": 2 }\r\n        ]\r\n      },\r\n      {\r\n        \"name\": \"Related Device 3\",\r\n        \"desc\": \"Oh yeah I need one of these too\",\r\n        \"price\": 179.99,\r\n        \"image\": \"https://picsum.photos/seed/relateditem3/300\",\r\n        \"stores\": [\r\n          { \"name\": \"Lancaster\", \"stock\": 0 },\r\n          { \"name\": \"Grove City\", \"stock\": 3 },\r\n          { \"name\": \"Canal Winchester\", \"stock\": 5 }\r\n        ]\r\n      }\r\n    ]\r\n  }\r\n}\r\n");
    }
}