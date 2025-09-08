using ClassLibrary.Entities;
using ClassLibrary.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;

namespace YoutubeDemos;

public class MCPTools(ILogger<MCPTools> _logger, CustomerSupportService customerSupportService)
{
    private readonly string fileName = "customer_support";
    [Function("HelloWorldTool")]
    public string Run([McpToolTrigger("hello-world", "This is a hello world tool")] ToolInvocationContext toolInvocationContext)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return "Welcome to Azure Functions!";
    }
    [Function("GetSupportData")]
    public async Task<List<CustomerSupportEntry>> GetSupportData([McpToolTrigger("get-support-data", "This tool returns a list of all support questions and their answers.")] ToolInvocationContext toolInvocationContext)
    {
        _logger.LogInformation("Get a list of support questions and answers.");
        return await customerSupportService.GetSupportData(fileName);
    }
    [Function("GetPromptCompletion")]
    public async Task<string> GetPromptCompletion([McpToolTrigger("get-prompt-completion", "This tool returns an answer based on the prompt.")] ToolInvocationContext toolInvocationContext
        , [McpToolProperty("Prompt", "string", "Gets a prompt completion based on the prompt")] string prompt)
    {
        _logger.LogInformation("Get a list of support questions and answers.");
        return await customerSupportService.GetPromptCompletion(prompt, fileName);
    }
}