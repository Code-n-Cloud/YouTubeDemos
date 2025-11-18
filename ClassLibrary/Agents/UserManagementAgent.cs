using Azure.AI.Agents.Persistent;
using ClassLibrary.Entities;
using Microsoft.Extensions.Configuration;

namespace ClassLibrary.Agents
{
    public class UserManagementAgent
    {
        private readonly PersistentAgentsClient persistentAgentsClient;
        public UserManagementAgent(IConfiguration configuration)
        {
            string foundryEndpoint = configuration["AZURE_FOUNDRY_ENDPOINT"];
            persistentAgentsClient = new PersistentAgentsClient(foundryEndpoint, new Azure.Identity.DefaultAzureCredential());
        }
        public async Task<string> CreateUserAccount(UserAccountModel createNewUserAccountModel)
        {
            string prompt = $"""
                Create a new user account with the following details:
                Full Name: {createNewUserAccountModel.FullName}
                Email Address: {createNewUserAccountModel.EmailAddress}
                Phone Number: {createNewUserAccountModel.PhoneNumber}
                Password: {createNewUserAccountModel.Password}
                """;
            string agentResponse = await CallUserManagementService(prompt);
            return agentResponse;
        }
        public async Task<string> GetAllUsers()
        {
            string prompt = "Please give me a list of all users and make sure you return me raw json. Please don't add any extra text or formatting. I have to parse the returned json.";
            string agentResponse = await CallUserManagementService(prompt);
            return agentResponse;
        }
        public async Task<string> GetUser(string emailAddress, string password)
        {
            string prompt = $"Please give me a single user based on the email {emailAddress} and password {password} and make sure you return me raw json. If no user found please return me an empty string. Please don't add any extra text or formatting. I have to parse the returned json.";
            string agentResponse = await CallUserManagementService(prompt);
            return agentResponse;
        }
        private async Task<string> CallUserManagementService(string prompt)
        {
            string agentId = "asst_ZdmMAV9G6wCQiHte4KGbDtlx";
            PersistentAgentThread agentThread = persistentAgentsClient.Threads.CreateThread();
            var agent = persistentAgentsClient.Administration.GetAgent(agentId);
            var message = persistentAgentsClient.Messages.CreateMessage(agentThread.Id, MessageRole.User, prompt);
            var threadRun = persistentAgentsClient.Runs.CreateRun(agentThread.Id, agent.Value.Id);
            do
            {
                await Task.Delay(2000);
                threadRun = persistentAgentsClient.Runs.GetRun(agentThread.Id, threadRun.Value.Id);
                if (threadRun.Value.Status == RunStatus.RequiresAction)
                {
                    if (threadRun.Value.RequiredAction is SubmitToolApprovalAction toolApprovalAction)
                    {
                        var approvedToolCalls = new List<ToolApproval>();
                        foreach (var toolCall in toolApprovalAction.SubmitToolApproval.ToolCalls)
                        {
                            if (toolCall is RequiredMcpToolCall mcpToolCall)
                            {
                                approvedToolCalls.Add(new ToolApproval(toolCall.Id, true));
                            }
                        }
                        await persistentAgentsClient.Runs.SubmitToolOutputsToRunAsync(agentThread.Id, threadRun.Value.Id, null, approvedToolCalls);
                    }
                }
                if (threadRun.Value.Status == RunStatus.Completed)
                {
                    break;
                }
            } while (threadRun.Value.Status == RunStatus.Queued || threadRun.Value.Status == RunStatus.InProgress || threadRun.Value.Status == RunStatus.RequiresAction);

            var agentMessages = persistentAgentsClient.Messages.GetMessagesAsync(agentThread.Id);
            await foreach (var agentMessage in agentMessages)
            {
                foreach (var messageContent in agentMessage.ContentItems)
                {
                    if (messageContent is MessageTextContent textContent)
                    {
                        return textContent.Text;
                    }
                }
            }
            return "";
        }
    }
}
