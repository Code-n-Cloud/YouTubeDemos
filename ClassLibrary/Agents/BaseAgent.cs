using Azure.AI.Agents.Persistent;

namespace ClassLibrary.Agents
{
    public class BaseAgent
    {
        private readonly PersistentAgentsClient persistentAgentsClient;
        public BaseAgent(PersistentAgentsClient persistentAgentsClient)
        {
            this.persistentAgentsClient = persistentAgentsClient;
        }
        public async Task<string> CallFoundryAgent(string prompt, string agentId)
        {
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
