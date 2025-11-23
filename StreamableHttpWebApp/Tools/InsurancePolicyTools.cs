using ClassLibrary.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace StreamableHttpWebApp.Tools
{
    public class InsurancePolicyTools(StorageAccountBlobService storageAccountBlobService, ILogger<InsurancePolicyTools> logger)
    {
        [McpServerTool, Description("Creates new insurance policy")]
        public async Task<string> CreateNewInsurancePolicy(
            [Description("Content of the policy")] string content,
            [Description("Insurer of the policy")] string insurer,
            [Description("Title of the policy")] string title,
            [Description("Tags for the policy, comma seperated string")] string tags,
            [Description("PremiumAmount of the policy")] double premiumAmount,
            [Description("IsActive, described if policy is active or not")] bool isActive)
        {
            try
            {
                var document = new ClassLibrary.Entities.AzureSearchDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = content,
                    Insurer = insurer,
                    Title = title,
                    Tags = tags.Split(',').Select(t => t.Trim()).ToArray(),
                    PremiumAmount = premiumAmount,
                    IsActive = isActive
                };
                await storageAccountBlobService.UplaodDocuments([document]);
                return "Insurance policy created successfully with ID: " + document.Id;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating new user account for");
                throw;
            }
        }
    }
}
