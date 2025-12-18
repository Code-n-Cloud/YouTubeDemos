using ModelContextProtocol.Server;
using SessionBasedFiltering.Services;
using SessionBasedFiltering.Tools;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SessionBasedFiltering
{
    public class Program
    {
        public static McpServerTool[] GetToolsForType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TTool>(IServiceProvider serviceProvider)
        {
            var tools = new List<McpServerTool>();
            //var instance = Activator.CreateInstance(typeof(TTool));
            var instance = ActivatorUtilities.CreateInstance<TTool>(serviceProvider);
            //var toolMethods = typeof(TTool).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var toolMethods = typeof(TTool).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(McpServerToolAttribute), false).Length > 0);
            foreach (var method in toolMethods)
            {
                var tool = McpServerTool.Create(method, instance, new McpServerToolCreateOptions());
                tools.Add(tool);
            }
            return [.. tools];
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<UserManagementService>();

            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<UserManagementTools>();
            //builder.Services.AddMcpServer().WithHttpTransport().WithTools<InsuranceManagementTools>();

            //McpServerTool[] userManagementTools = [McpServerTool.Create(UserManagementTools.UserManagementTool_1, new McpServerToolCreateOptions())
            //    ,McpServerTool.Create(UserManagementTools.UserManagementTool_2, new McpServerToolCreateOptions())
            //    ,McpServerTool.Create(UserManagementTools.UserManagementTool_3, new McpServerToolCreateOptions())];

            //McpServerTool[] insuranceManagementTools = [McpServerTool.Create(InsuranceManagementTools.InsuranceManagementTool_1, new McpServerToolCreateOptions())
            //    ,McpServerTool.Create(InsuranceManagementTools.InsuranceManagementTool_2, new McpServerToolCreateOptions())
            //    ,McpServerTool.Create(InsuranceManagementTools.InsuranceManagementTool_3, new McpServerToolCreateOptions())
            //    ,McpServerTool.Create(InsuranceManagementTools.InsuranceManagementTool_4, new McpServerToolCreateOptions())];

            var allTools = new ConcurrentDictionary<string, McpServerTool[]>();



            builder.Services.AddMcpServer().WithHttpTransport(option =>
            {
                option.ConfigureSessionOptions = async (HttpContext httpContext, McpServerOptions mcpServerOptions, CancellationToken cancellationToken) =>
                {
                    var routeCategory = httpContext.Request.RouteValues["category"]?.ToString()?.ToLower() ?? "all";
                    allTools.TryGetValue(routeCategory, out var selectedTools);
                    mcpServerOptions.ToolCollection = [.. selectedTools ?? []];
                    await Task.CompletedTask;
                };
            });

            var app = builder.Build();

            var userManagementTools = GetToolsForType<UserManagementTools>(app.Services);
            var insuranceManagementTools = GetToolsForType<InsuranceManagementTools>(app.Services);
            allTools.TryAdd("usermanagementtools", userManagementTools);
            allTools.TryAdd("insurancemanagementtools", insuranceManagementTools);
            allTools.TryAdd("all", [.. userManagementTools, .. insuranceManagementTools]);

            app.MapGet("/health", () => "Hello World! health");

            app.MapMcp("/{category}");

            app.Run();
        }
    }
}
