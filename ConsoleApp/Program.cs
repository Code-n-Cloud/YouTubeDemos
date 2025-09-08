using ClassLibrary.Services;
using ConsoleApp.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateEmptyApplicationBuilder(settings: null);
            builder.Services.AddMcpServer().WithStdioServerTransport().WithTools<SmartHomeTools>();
            builder.Services.AddSingleton<CustomerSupportService>();
            var app = builder.Build();
            app.Run();
        }
    }
}
