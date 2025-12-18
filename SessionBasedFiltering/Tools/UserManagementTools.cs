using ModelContextProtocol.Server;
using SessionBasedFiltering.Services;
using System.ComponentModel;

namespace SessionBasedFiltering.Tools
{
    public class UserManagementTools(UserManagementService userManagementService)
    {
        [McpServerTool, Description("UserManagementTool_1 from SessionBasedFiltering")]
        public string UserManagementTool_1()
        {
            return userManagementService.GetUserManagementInfo();
        }
        [McpServerTool, Description("UserManagementTool_1 from SessionBasedFiltering")]
        public string UserManagementTool_2()
        {
            return "UserManagementTool_2 from SessionBasedFiltering";
        }
        [McpServerTool, Description("UserManagementTool_3 from SessionBasedFiltering")]
        public string UserManagementTool_3()
        {
            return "UserManagementTool_3 from SessionBasedFiltering";
        }
        [McpServerTool, Description("UserManagementTool_4 from SessionBasedFiltering")]
        public string UserManagementTool_4()
        {
            return "UserManagementTool_4 from SessionBasedFiltering";
        }
        public string NonToolMethod()
        {
            return "This method is not a tool and should not be exposed.";
        }
    }
}
