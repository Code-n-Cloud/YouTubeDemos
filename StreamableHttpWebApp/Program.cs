using Azure;
using Azure.Data.Tables;
using Azure.Search.Documents;
using Azure.Storage.Blobs;
using ClassLibrary.Services;
using Microsoft.Extensions.Azure;
using ModelContextProtocol.Server;
using OpenAI;
using StreamableHttpWebApp.Tools;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Reflection;

namespace StreamableHttpWebApp
{
    public class Program
    {
        static McpServerTool[] GetToolsForType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(IServiceProvider serviceProvider)
        {
            var tools = new List<McpServerTool>();
            var instance = ActivatorUtilities.CreateInstance<T>(serviceProvider);
            var toolMethods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance)


                .Where(x => x.GetCustomAttributes(typeof(McpServerToolAttribute), false).Any());


            foreach (var method in toolMethods)
            {
                var tool = McpServerTool.Create(method, instance, new McpServerToolCreateOptions());
                tools.Add(tool);
            }
            return [.. tools];
        }
        static string GetConfigurationValue(WebApplicationBuilder webApplicationBuilder, string key)
        {
            var configurationValue = webApplicationBuilder.Configuration[key];
            if (string.IsNullOrEmpty(configurationValue))
            {
                throw new InvalidOperationException($"Configuration value for '{key}' is missing.");
            }
            return configurationValue;
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<AzureSearchTools>();
            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<UtilityTools>();
            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<UserManagementTools>();
            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<InsurancePolicyTools>();
            var toolsDictionary = new ConcurrentDictionary<string, McpServerTool[]>();

            builder.Services.AddMcpServer().WithHttpTransport(options =>
            {
                options.ConfigureSessionOptions = async (HttpContext httpContext, McpServerOptions mcpServerOptions, CancellationToken cancellationToken) =>
                {
                    var toolCategory = httpContext.Request.RouteValues["category"]?.ToString()?.ToLower() ?? "all";
                    toolsDictionary.TryGetValue(toolCategory, out McpServerTool[] tools);
                    mcpServerOptions.ToolCollection = new McpServerPrimitiveCollection<McpServerTool>();
                    foreach (var tool in tools!)
                    {
                        mcpServerOptions.ToolCollection.Add(tool);
                    }
                    await Task.CompletedTask;
                };
            });


            builder.Services.AddSingleton<StorageAccountBlobService>();
            builder.Services.AddSingleton<UserManagementService>();
            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<WeatherAlertsTool>();
            builder.Services.AddHttpClient("WeatherApi", client =>
                {
                    client.BaseAddress = new Uri("https://api.weather.gov/");
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-tool", "1.0"));
                });

            builder.Services.AddTransient(serviceProvider =>
            {
                string openAiKey = GetConfigurationValue(builder, "AZURE_OPENAI_KEY");
                string openAiEndpoint = GetConfigurationValue(builder, "AZURE_OPENAI_ENDPOINT");
                var client = new OpenAIClient(new Azure.AzureKeyCredential(openAiKey)
               , new OpenAIClientOptions() { Endpoint = new Uri($"{openAiEndpoint}/models") });
                return client.GetEmbeddingClient("text-embedding-3-small");
            });
            builder.Services.AddTransient(serviceProvider =>
            {
                string openSearchKey = GetConfigurationValue(builder, "AZURE_SEARCH_KEY");
                string openSearchEndpoint = GetConfigurationValue(builder, "AZURE_SEARCH_ENDPOINT");
                return new SearchClient(new Uri(openSearchEndpoint), "insurance-documents-index", new AzureKeyCredential(openSearchKey));
            });
            builder.Services.AddTransient(serviceProvider =>
            {
                var connectionString = GetConfigurationValue(builder, "AzureWebJobsStorage");
                return new BlobContainerClient(connectionString, "rag-documents");
            });


            builder.Services.AddAzureClients(azureBuilder =>
            {
                var connectionString = GetConfigurationValue(builder, "AzureWebJobsStorage");
                azureBuilder.AddTableServiceClient(connectionString);
            });

            builder.Services.AddSingleton(provider =>
            {
                var serviceClient = provider.GetRequiredService<TableServiceClient>();
                var tableClient = serviceClient.GetTableClient("UserAccounts");
                return tableClient;
            });
            var app = builder.Build();
            McpServerTool[] weatherForecastTools = GetToolsForType<WeatherAlertsTool>(app.Services);
            McpServerTool[] azureSearchTools = GetToolsForType<AzureSearchTools>(app.Services);
            McpServerTool[] insuranceClaimTools = GetToolsForType<InsuranceClaimTools>(app.Services);
            toolsDictionary.TryAdd("weatherforecasttools", weatherForecastTools);
            toolsDictionary.TryAdd("azuresearchtools", azureSearchTools);
            toolsDictionary.TryAdd("insuranceclaimtools", insuranceClaimTools);
            toolsDictionary.TryAdd("all", [.. weatherForecastTools, .. azureSearchTools, .. insuranceClaimTools]);

            app.MapMcp("/{category}");
            app.MapGet("/health", () => "Streamable HTTP Web App is running. Use /mcp to access the MCP tools.");
            app.Run();
        }
    }
}
