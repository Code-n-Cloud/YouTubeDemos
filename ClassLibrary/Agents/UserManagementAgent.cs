using Azure.AI.Agents.Persistent;
using ClassLibrary.Entities;

namespace ClassLibrary.Agents
{
    public class UserManagementAgent
    {
        private readonly PersistentAgentsClient persistentAgentsClient;
        private readonly string agentId;
        public UserManagementAgent(PersistentAgentsClient persistentAgentsClient, string agentId)
        {
            this.persistentAgentsClient = persistentAgentsClient;
            this.agentId = agentId;
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
            string agentResponse = await new BaseAgent(persistentAgentsClient).CallFoundryAgent(prompt, agentId);
            return agentResponse;
        }
        public async Task<string> GetAllUsers()
        {
            string prompt = "Please give me a list of all users and make sure you return me raw json. Please don't add any extra text or formatting. I have to parse the returned json.";
            string agentResponse = await new BaseAgent(persistentAgentsClient).CallFoundryAgent(prompt, agentId);
            return agentResponse;
        }
        public async Task<string> GetUser(string emailAddress, string password)
        {
            string prompt = $"Please give me a single user based on the email {emailAddress} and password {password} and make sure you return me raw json. If no user found please return me an empty string. Please don't add any extra text or formatting. I have to parse the returned json.";
            string agentResponse = await new BaseAgent(persistentAgentsClient).CallFoundryAgent(prompt, agentId);
            return agentResponse;
        }
        public async Task<string> GetUserName()
        {
            return "John Smith";
        }
    }
}
