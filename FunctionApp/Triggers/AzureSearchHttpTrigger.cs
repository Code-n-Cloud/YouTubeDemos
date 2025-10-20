using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using ClassLibrary.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunctionApp.Triggers;

public class AzureSearchHttpTrigger
{
    private readonly ILogger<AzureSearchHttpTrigger> _logger;
    private readonly DocumentService _documentService;

    public AzureSearchHttpTrigger(ILogger<AzureSearchHttpTrigger> logger, DocumentService documentService)
    {
        _logger = logger;
        _documentService = documentService;
    }

    [Function("AzureSearchHttpTrigger")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string KeywordSearchQuery = req.Query["KeywordSearchQuery"];
        string VectorSearchQuery = req.Query["VectorSearchQuery"];

        SearchOptions options = new SearchOptions()
        {
            IncludeTotalCount = true,
            Size = int.TryParse(req.Query["Size"], out int size) ? size : 50
        };
        if (!string.IsNullOrEmpty(VectorSearchQuery))
        {
            var vectorQuery = new VectorizedQuery(await _documentService.GetEmbeddings(VectorSearchQuery))
            {
                Fields = { "ContentVector" },
                KNearestNeighborsCount = 20
            };
            options.VectorSearch = new() { Queries = { vectorQuery } };
        }

        string columns = req.Query["Columns"];
        if (!string.IsNullOrEmpty(columns))
        {
            var cols = columns.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var col in cols)
            {
                options.Select.Add(col);
            }
        }
        string filter = req.Query["Filter"];
        if (!string.IsNullOrEmpty(filter))
        {
            options.Filter = filter;
        }
        string sort = req.Query["Sort"];
        if (!string.IsNullOrEmpty(sort))
        {
            options.OrderBy.Add(sort);
        }
        string facets = req.Query["Facets"];
        if (!string.IsNullOrEmpty(facets))
        {
            options.Facets.Add(facets);
        }

        var searchResult = await _documentService.SearchDocuments(KeywordSearchQuery, options);

        var documents = searchResult.Value.GetResults();

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(new
        {
            totalHits = searchResult.Value.TotalCount,
            returnedTotal = documents.Count(),
            factes = searchResult.Value.Facets?.SelectMany(x => x.Value.Select(y => new { key = y.Value, count = y.Count })),
            results = documents
        }, new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull }));
        return response;
    }
}