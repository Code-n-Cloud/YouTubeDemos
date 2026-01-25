using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace SecuredTools.Tools
{
    [McpServerToolType]
    public class WeatherTools
    {
        [McpServerTool, Description("Public tool 1")]
        public static string PublicWeatherTool1(string message)
        {
            return $"PublicWeatherTool1 received: {message}";
        }
        [McpServerTool, Description("Public tool 2")]
        public static string PublicWeatherTool2(string message)
        {
            return $"PublicWeatherTool1 received: {message}";
        }
        [Authorize(Policy = "WeatherSecured")]
        [McpServerTool, Description("Secured tool 3")]
        public static string SecuredWeatherTool3(string message)
        {
            return $"PublicWeatherTool1 received: {message}";
        }
    }
}
