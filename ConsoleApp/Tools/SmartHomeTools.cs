using ClassLibrary.Entities;
using ClassLibrary.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace ConsoleApp.Tools
{
    [McpServerToolType]
    public class SmartHomeTools(CustomerSupportService customerSupportService)
    {
        private readonly string fileName = "smart_home";
        [McpServerTool, Description("Get the current status of the smart home system.")]
        public string GetSmartHomeStatus()
        {
            return "All systems operational.";
        }
        [McpServerTool, Description("Get a list of smart home system questions.")]
        public async Task<List<CustomerSupportEntry>> GetSmartHomeData()
        {
            return await customerSupportService.GetSupportData(fileName);
        }
        [McpServerTool, Description("Get the propmp completion for the smart home system question.")]
        public async Task<string> GetSmartHomeDataPromptCompletion(string prompt)
        {
            return await customerSupportService.GetPromptCompletion(prompt, fileName);
        }
    }
}
