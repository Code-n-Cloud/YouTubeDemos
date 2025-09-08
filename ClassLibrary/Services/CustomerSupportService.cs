using ClassLibrary.Entities;
using System.Text.Json;

namespace ClassLibrary.Services
{
    public class CustomerSupportService
    {
        public async Task<List<CustomerSupportEntry>> GetSupportData(string fileName)
        {
            return await GetCustomerSupportEntries(fileName);
        }
        public async Task<string> GetPromptCompletion(string prompt, string fileName)
        {
            List<CustomerSupportEntry> entries = await GetCustomerSupportEntries(fileName);
            return entries.FirstOrDefault(e => e.Prompt.Contains(prompt, StringComparison.OrdinalIgnoreCase))?.Completion
                   ?? "Sorry, I don't have an answer for that question.";
        }
        private async Task<List<CustomerSupportEntry>> GetCustomerSupportEntries(string fileName)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            using var httpClient = new HttpClient();

            var response = await httpClient.GetAsync($"https://styoutubedemos.blob.core.windows.net/mcp-data/{fileName}.json");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CustomerSupportEntry>>(jsonString, options);
        }
    }
}
