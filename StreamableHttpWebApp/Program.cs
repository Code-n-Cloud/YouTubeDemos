using Azure.Data.Tables;
using ClassLibrary.Services;
using Microsoft.Extensions.Azure;
using StreamableHttpWebApp.Tools;

namespace StreamableHttpWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<AzureSearchTools>();
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<UtilityTools>();
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<UserManagementTools>();

            builder.Services.AddSingleton<DocumentService>();
            builder.Services.AddSingleton<UserManagementService>();
            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<WeatherAlertsTool>();
            //builder.Services.AddHttpClient("WeatherApi", client =>
            //    {
            //        client.BaseAddress = new Uri("https://api.weather.gov/");
            //        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-tool", "1.0"));
            //    });

            builder.Services.AddAzureClients(azureBuilder =>
            {
                var configuration = builder.Configuration;
                var connectionString = configuration["AzureWebJobsStorage"];
                azureBuilder.AddTableServiceClient(connectionString);
            });

            builder.Services.AddSingleton(provider =>
            {
                var serviceClient = provider.GetRequiredService<TableServiceClient>();
                var tableClient = serviceClient.GetTableClient("UserAccounts");
                return tableClient;
            });
            var app = builder.Build();
            app.MapMcp();
            app.MapGet("/", () => "Welcome to the Streamable HTTP Web App. Use /mcp to access the MCP tools.");
            app.MapGet("/health", () => "Streamable HTTP Web App is running. Use /mcp to access the MCP tools.");
            app.Run();
        }
    }
}
