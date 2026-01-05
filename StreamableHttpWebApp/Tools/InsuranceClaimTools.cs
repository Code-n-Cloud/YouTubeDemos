using ClassLibrary.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace StreamableHttpWebApp.Tools
{
    public class InsuranceClaimTools(StorageAccountBlobService storageAccountBlobService, ILogger<InsuranceClaimTools> logger, TextToSpeechService textToSpeechService)
    {
        [McpServerTool, Description("Creates new insurance policy")]
        public async Task<string> CreateNewInsurancePolicy(
            [Description("Content of the policy")] string content,
            [Description("Insurer of the policy")] string insurer,
            [Description("Title of the policy")] string title,
            [Description("Tags for the policy, comma separated string")] string tags,
            [Description("PremiumAmount of the policy")] double premiumAmount,
            [Description("IsActive, described if policy is active or not")] bool isActive,
            [Description("Text of the speech video")] string speechText)
        {
            try
            {
                string Id = Guid.NewGuid().ToString();
                var document = new ClassLibrary.Entities.AzureSearchDocument
                {
                    Id = Id,
                    Content = content,
                    Insurer = insurer,
                    Title = title,
                    Tags = tags.Split(',').Select(t => t.Trim()).ToArray(),
                    PremiumAmount = premiumAmount,
                    IsActive = isActive,
                    SpeechText = speechText
                };
                await storageAccountBlobService.UplaodDocuments([document]);
                await textToSpeechService.ProcessSpeechToText(Id, speechText);
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
