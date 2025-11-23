using Azure.AI.Agents.Persistent;
using ClassLibrary.Entities;

namespace ClassLibrary.Agents
{
    public class InsuranceClaimAgent
    {
        private readonly PersistentAgentsClient persistentAgentsClient;
        private readonly string agentId;
        public InsuranceClaimAgent(PersistentAgentsClient persistentAgentsClient, string agentId)
        {
            this.persistentAgentsClient = persistentAgentsClient;
            this.agentId = agentId;
        }
        public async Task<string> CreateIsuranceClaim(InsuranceClaimModel insuranceClaimModel)
        {
            string prompt = $"""
                Create a new insurance claim with the following details:
                Content: {insuranceClaimModel.Content}
                Insurer: {insuranceClaimModel.Insurer}
                Title: {insuranceClaimModel.Title}
                tags: {insuranceClaimModel.Tags.FirstOrDefault()}
                premiumAmount: {insuranceClaimModel.PremiumAmount}
                isActive: {insuranceClaimModel.IsActive}
                """;
            string agentResponse = await new BaseAgent(persistentAgentsClient).CallFoundryAgent(prompt, agentId);
            return agentResponse;
        }
    }
}
