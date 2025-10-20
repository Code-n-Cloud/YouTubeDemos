using ClassLibrary.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Triggers;

public class AzureSearchDocumentsUploader
{
    private readonly ILogger<AzureSearchDocumentsUploader> _logger;

    private readonly DocumentService _documentService;
    public AzureSearchDocumentsUploader(ILogger<AzureSearchDocumentsUploader> logger, DocumentService documentService)
    {
        _logger = logger;
        _documentService = documentService;
    }

    [Function("AzureSearchDocumentsUploader")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        //var docs = _documentService.GenerateHealthInsuranceDocuments(100);
        //await _documentService.UplaodDocuments(docs);

        //await _documentService.CreateOrUpdateIndex();
        await _documentService.CreateOrUpdateIndexer();

        return req.CreateResponse(System.Net.HttpStatusCode.OK);
    }
}