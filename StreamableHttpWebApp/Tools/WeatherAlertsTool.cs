using ClassLibrary.Entities;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace StreamableHttpWebApp.Tools
{
    public class WeatherAlertsTool(IHttpClientFactory httpClientFactory)
    {
        [McpServerTool, Description("Hello world from Streamable Http tool")]
        public string GetHelloWorld()
        {
            return "Hello world from Streamable Http tool.";
        }
        [McpServerTool, Description("This tool returns alerts fromt the https://api.weather.gov/ API based on the state code.")]
        public async Task<List<WeatherAlert>> GetAlerts([Description("2 characters state code for example NY")] string stateCode)
        {
            var client = httpClientFactory.CreateClient("WeatherApi");
            using var response = await client.GetStreamAsync($"/alerts?area={stateCode}&limit=10");
            using var doc = await JsonDocument.ParseAsync(response) ?? throw new McpException("No JSON returned from the alerts endpoint");
            var features = doc.RootElement.GetProperty("features").EnumerateArray();
            if (features.Any() == false)
            {
                return [];
            }
            var alerts = features.Select(f => new WeatherAlert
            {
                Event = f.GetProperty("properties").GetProperty("event").GetString() ?? string.Empty,
                AreaDesc = f.GetProperty("properties").GetProperty("areaDesc").GetString() ?? string.Empty,
                Severity = f.GetProperty("properties").GetProperty("severity").GetString() ?? string.Empty,
                Description = f.GetProperty("properties").GetProperty("description").GetString() ?? string.Empty
            }).ToList();
            return alerts;
        }
    }
}
