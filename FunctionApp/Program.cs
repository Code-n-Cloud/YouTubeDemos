using Azure.AI.Agents.Persistent;
using ClassLibrary.Agents;
using ClassLibrary.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddSingleton<StorageAccountBlobService>();

builder.Services.AddSingleton(serviceProvider =>
{
    string foundryEndpoint = GetConfigurationValue("AZURE_FOUNDRY_ENDPOINT");
    return new PersistentAgentsClient(foundryEndpoint, new Azure.Identity.DefaultAzureCredential());
});

builder.Services.AddSingleton(serviceProvider =>
{
    string foundryEndpoint = GetConfigurationValue("AZURE_FOUNDRY_ENDPOINT");
    string agentId = GetConfigurationValue("INSURANCE_CLAIM_AGENT_ID");
    var client = serviceProvider.GetRequiredService<PersistentAgentsClient>();
    return new InsuranceClaimAgent(client, agentId);
});
string GetConfigurationValue(string key)
{
    var configurationValue = builder.Configuration[key];
    if (string.IsNullOrEmpty(configurationValue))
    {
        throw new InvalidOperationException($"Configuration value for '{key}' is missing.");
    }
    return configurationValue;
}

//builder.Services
//    .AddApplicationInsightsTelemetryWorkerService()
//    .ConfigureFunctionsApplicationInsights();
//builder.Services.AddSingleton<CustomerSupportService>();
//builder.Services.AddSingleton<StorageAccountService>();

//builder.Services.AddAzureClients(azureBuilder =>
//{
//    var configuration = builder.Configuration;
//    var connectionString = configuration["AzureWebJobsStorage"];
//    azureBuilder.AddTableServiceClient(connectionString);
//});

//builder.Services.AddSingleton(provider =>
//{
//    var serviceClient = provider.GetRequiredService<TableServiceClient>();
//    var tableClient = serviceClient.GetTableClient("CustomerHistory");
//    return tableClient;
//});
builder.Build().Run();
