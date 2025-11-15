using ClassLibrary.Agents;
using ClassLibrary.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace WebApplication1.Pages
{
    public class GetAllUsersModel : PageModel
    {
        public List<UserAccountModel> Users { get; set; } = new();
        private readonly UserManagementAgent userManagementAgent;
        public GetAllUsersModel(UserManagementAgent userManagementAgent)
        {
            this.userManagementAgent = userManagementAgent;
        }
        public async Task OnGet()
        {
            var usersResponse = await userManagementAgent.GetAllUsers();
            Users = JsonSerializer.Deserialize<List<UserAccountModel>>(usersResponse, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
