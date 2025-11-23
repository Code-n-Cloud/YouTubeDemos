using ClassLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

namespace FunctionApp.Triggers;

public class CustomerHistory(StorageAccountTableService storageAccountService, ILogger<CustomerHistory> logger)
{

    [Function("CustomerHistory")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        try
        {
            string email = req.Query["email"];
            JsonObject history = await storageAccountService.GetCustomerHistoryEntity(email, email);
            if (history == null)
            {
                return new OkObjectResult(new { });
            }
            return new OkObjectResult(history);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
    }
}