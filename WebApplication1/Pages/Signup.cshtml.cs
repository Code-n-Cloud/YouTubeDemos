using ClassLibrary.Agents;
using ClassLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty]
        public CreateNewUserAccountModel UserInput { get; set; } = new();
        private readonly UserManagementAgent userManagementAgent;
        public SignupModel(UserManagementAgent userManagementAgent)
        {
            this.userManagementAgent = userManagementAgent;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await userManagementAgent.CreateUserAccount(UserInput);
                TempData["SuccessMessage"] = "User account created successfully!";
                return RedirectToPage("/Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating user: {ex.Message}");
                return Page();
            }
        }
    }
}
