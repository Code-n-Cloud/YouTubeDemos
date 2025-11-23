using ClassLibrary.Agents;
using ClassLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class NewInsurancePolicyModel : PageModel
    {
        private readonly InsuranceClaimAgent insuranceClaimAgent;
        public NewInsurancePolicyModel(InsuranceClaimAgent insuranceClaimAgent)
        {
            this.insuranceClaimAgent = insuranceClaimAgent;
        }

        [BindProperty]
        public InsuranceClaimModel InsurancePolicyModel { get; set; } = new InsuranceClaimModel();
        public void OnGet()
        {
        }
        public async Task OnPost()
        {
            await insuranceClaimAgent.CreateIsuranceClaim(this.InsurancePolicyModel);
        }
    }
}
