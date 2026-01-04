using ModelContextProtocol.Server;
using System.ComponentModel;

namespace SessionBasedFiltering.Tools
{
    public class InsuranceManagementTools
    {
        [McpServerTool, Description("InsuranceManagementTool_1 from SessionBasedFiltering")]
        public string InsuranceManagementTool_1()
        {
            return "InsuranceManagementTool_1 from SessionBasedFiltering";
        }
        [McpServerTool, Description("InsuranceManagementTool_2 from SessionBasedFiltering")]
        public string InsuranceManagementTool_2()
        {
            return "InsuranceManagementTool_2 from SessionBasedFiltering";
        }
        [McpServerTool, Description("InsuranceManagementTool_3 from SessionBasedFiltering")]
        public string InsuranceManagementTool_3()
        {
            return "InsuranceManagementTool_3 from SessionBasedFiltering";
        }
        [McpServerTool, Description("InsuranceManagementTool_4 from SessionBasedFiltering")]
        public string InsuranceManagementTool_4()
        {
            return "InsuranceManagementTool_4 from SessionBasedFiltering";
        }
    }
}
