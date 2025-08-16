using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;

namespace Text;

public class TextFunction
{
    private static readonly CosmosClient cosmosClient = new CosmosClient("https://exnosqlcosmosdb.documents.azure.com:443/", "");//(CosmosDbConfig.DbEndpointUri, CosmosDbConfig.DbPrimaryKey);

    private readonly ILogger<TextFunction> _logger;

    public TextFunction(ILogger<TextFunction> logger)
    {
        _logger = logger;
    }

    [Function("Text")]
    public static async Task<HttpResponseData> Text([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
    FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("HttpExample");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var message = "Welcome to Azure Functions!";

        try
        {
            var container = cosmosClient.GetContainer("bookmarks", "Bookmarks");
            string id = "docs";
            ItemResponse<dynamic> responseItem = await container.ReadItemAsync<dynamic>(id, new PartitionKey(id));
            var response = req.CreateResponse(HttpStatusCode.OK);
            string responseData = JsonConvert.SerializeObject(responseItem.Resource);
            await response.WriteStringAsync(responseData);
            return response;
        }
        catch (CosmosException e)
        {
            var errorReponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorReponse.WriteStringAsync($"Error Occured: {e.Message}");
            return errorReponse;
        }
    }

    public class MultiResponse
    {
        [CosmosDBOutput("bookmarks","Bookmarks",//"my-database", "my-container",
            Connection = "CosmosDbConnectionSetting", CreateIfNotExists = true)]
        public MyDocument Document { get; set; }
        public HttpResponseData HttpResponse { get; set; }
    }
    public class MyDocument
    {
        public string id { get; set; }
        public string message { get; set; }
    }
}