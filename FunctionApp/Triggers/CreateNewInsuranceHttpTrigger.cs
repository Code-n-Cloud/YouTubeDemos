using ClassLibrary.Agents;
using ClassLibrary.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FunctionApp.Triggers;

public class CreateNewInsuranceHttpTrigger
{
    private readonly ILogger<CreateNewInsuranceHttpTrigger> _logger;
    private readonly InsuranceClaimAgent insuranceClaimAgent;

    public CreateNewInsuranceHttpTrigger(ILogger<CreateNewInsuranceHttpTrigger> logger, InsuranceClaimAgent insuranceClaimAgent)
    {
        _logger = logger;
        this.insuranceClaimAgent = insuranceClaimAgent;
    }

    [Function("CreateNewInsuranceHttpTrigger")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var input = await JsonSerializer.DeserializeAsync<InsuranceClaimModel>(req.Body, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var agentResponse = await insuranceClaimAgent.CreateIsuranceClaim(input);


        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonSerializer.Serialize(new { message = agentResponse }));
        return response;
    }
}