using Azure.AI.Agents.Persistent;
using ClassLibrary.Agents;

namespace WebApplication1
{
    public class Program
    {
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

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddSingleton(serviceProvider =>
            {
                string foundryEndpoint = GetConfigurationValue(builder, "AZURE_FOUNDRY_ENDPOINT");
                string agentId = GetConfigurationValue(builder, "USERS_MANAGEMENT_AGENT_ID");
                var client = serviceProvider.GetRequiredService<PersistentAgentsClient>();
                return new UserManagementAgent(client, agentId);
            });

            builder.Services.AddSingleton(serviceProvider =>
            {
                string foundryEndpoint = GetConfigurationValue(builder, "AZURE_FOUNDRY_ENDPOINT");
                string agentId = GetConfigurationValue(builder, "INSURANCE_CLAIM_AGENT_ID");
                var client = serviceProvider.GetRequiredService<PersistentAgentsClient>();
                return new InsuranceClaimAgent(client, agentId);
            });

            builder.Services.AddSingleton(serviceProvider =>
            {
                string foundryEndpoint = GetConfigurationValue(builder, "AZURE_FOUNDRY_ENDPOINT");
                return new PersistentAgentsClient(foundryEndpoint, new Azure.Identity.DefaultAzureCredential());
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
