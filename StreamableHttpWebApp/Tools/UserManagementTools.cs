using ClassLibrary.Entities;
using ClassLibrary.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace StreamableHttpWebApp.Tools
{
    public class UserManagementTools(ILogger<UserManagementTools> logger, UserManagementService userManagementService)
    {
        [McpServerTool, Description("Creates new user account.")]
        public async Task<string> CreateNewUserAccount(
            [Description("Full name of the user, required field")] string fullName,
            [Description("Email address of the user, required field")] string emailAddress,
            [Description("Phone number of the user, required field")] string phoneNumber,
            [Description("Password of the user, required field")] string password)
        {
            try
            {
                return await userManagementService.CreateNewUserAccount(
                    fullName,
                    emailAddress,
                    phoneNumber,
                    password);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating new user account for");
                throw;
            }
        }
        [McpServerTool, Description("Gets a list of all users")]
        public async Task<List<UserAccountEntity>> GetAllUsers()
        {
            try
            {
                return await userManagementService.GetAllUsers();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in getting users list");
                throw;
            }
        }
        [McpServerTool, Description("Gets a signle user, or perform user sign in")]
        public async Task<UserAccountEntity> GetUser(
            [Description("Email address of the user, required field")] string emailAddress, [Description("Password of the user, required field")] string password)
        {
            try
            {
                return await userManagementService.GetUser(emailAddress, password);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in getting user");
                throw;
            }
        }
    }
}
