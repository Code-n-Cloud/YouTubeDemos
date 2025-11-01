using ClassLibrary.Services;
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
            builder.Services.AddSingleton<DocumentService>();
            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<WeatherAlertsTool>();
            //builder.Services.AddHttpClient("WeatherApi", client =>
            //    {
            //        client.BaseAddress = new Uri("https://api.weather.gov/");
            //        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weather-tool", "1.0"));
            //    });
            var app = builder.Build();
            app.MapMcp();
            app.MapGet("/health", () => "Streamable HTTP Web App is running. Use /mcp to access the MCP tools.");
            app.Run();
        }
    }
}
