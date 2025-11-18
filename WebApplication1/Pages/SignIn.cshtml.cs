using ClassLibrary.Agents;
using ClassLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace WebApplication1.Pages
{
    public class SignInModel : PageModel
    {
        [BindProperty]
        public UserSignInModel UserSignInModel { get; set; } = new UserSignInModel();
        private readonly UserManagementAgent userManagementAgent;

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public SignInModel(UserManagementAgent userManagementAgent)
        {
            this.userManagementAgent = userManagementAgent;
        }
        public void OnGet()
        {
        }
        public async Task OnPost()
        {
            var usersResponse = await userManagementAgent.GetUser(UserSignInModel.EmailAddress, UserSignInModel.Password);
            if (usersResponse == "\"\"")
            {
                ErrorMessage = "Invalid email or password.";
                return;
            }
            var user = JsonSerializer.Deserialize<UserAccountModel>(usersResponse, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            SuccessMessage = $"Welcome, {user.FullName}!";
        }
    }
}
