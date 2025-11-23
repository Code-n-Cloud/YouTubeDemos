using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using ClassLibrary.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace StreamableHttpWebApp.Tools
{
    public class AzureSearchTools(StorageAccountBlobService documentService)
    {
        [McpServerTool, Description("This tool performs vector search queries against Azure Search, enabling semantic content retrieval using Azure's Open API with optional OData filtering to limit results (e.g., by status or specific tags) and custom sorting to order the results by specified fields in ascending or descending order.")]
        public async Task<IEnumerable<SearchResult<SearchDocument>>> GetSearchResults(
            [Description("Vector search query")] string query,
            [Description("Optional OData filter expression to limit results, for example 'IsActive eq false and PremiumAmount gt 1500 and Tags/any(t:t eq 'hospitalization')")] string? filter = null,
            [Description("Optional field to specify sorting, for examaple 'PremiumAmount desc' or 'PremiumAmount asc'")] string? orderBy = null)
        {
            SearchOptions options = new SearchOptions()
            {
                Select = { "Id", "Content", "Insurer", "Title", "Tags", "PremiumAmount", "IsActive" }
            };
            if (string.IsNullOrEmpty(filter) == false)
            {
                options.Filter = filter;
            }
            if (string.IsNullOrEmpty(orderBy) == false)
            {
                options.OrderBy.Add(orderBy);
            }
            var searchResult = await documentService.SearchDocuments(query, options);
            var documents = searchResult.Value.GetResults();
            return documents;
        }
    }
}
