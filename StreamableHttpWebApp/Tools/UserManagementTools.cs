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
    }
}
