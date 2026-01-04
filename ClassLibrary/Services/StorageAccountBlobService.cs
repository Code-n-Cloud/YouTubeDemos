using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Bogus;
using ClassLibrary.Entities;
using OpenAI.Embeddings;
using System.Text.Json;

namespace ClassLibrary.Services
{
    public class StorageAccountBlobService
    {
        private string searchEndPoint;
        private string searchKey;
        private readonly EmbeddingClient embeddingClient;
        private readonly SearchClient searchClient;
        private readonly BlobContainerClient blobContainerClient;
        public StorageAccountBlobService(EmbeddingClient embeddingClient, SearchClient searchClient, BlobContainerClient blobContainerClient)
        {
            this.embeddingClient = embeddingClient;
            this.searchClient = searchClient;
            this.blobContainerClient = blobContainerClient;
        }
        public List<AzureSearchDocument> GenerateHealthInsuranceDocuments(int documentCount)
        {
            var coverageTypes = new List<string>() { "hospitalization", "prescription drugs", "denatl", "vision", "preventive care", "mental health", "maternity" };
            var exclusions = new List<string>() { "cosmetic procedures", "experimental treatments", "alternative therapies" };
            var extras = new List<string>() { "dental plan", "vision plan", "welness program" };
            var faker = new Faker<AzureSearchDocument>()
                .RuleFor(f => f.Id, p => Guid.NewGuid().ToString())
                .RuleFor(f => f.Title, (p, d) => $"Policy {d.Id}")
                .RuleFor(f => f.Insurer, p => $"{p.Company.CompanyName()} Insurance")
                .RuleFor(f => f.Tags, p => p.PickRandom(coverageTypes, 2).ToArray())
                .RuleFor(f => f.PremiumAmount, (p, d) => (double)p.Finance.Amount(500, 2500))
                .RuleFor(f => f.IsActive, p => p.Random.Bool())
                .RuleFor(f => f.Content, d =>
                {
                    var sentences = new List<string>()
                    {
                        $"This policy covers {d.PickRandom(coverageTypes)} for eligibal customers",
                        $"Optional extras include {d.PickRandom(extras)}",
                        $"Exclusions include {d.PickRandom(exclusions)} are not covered",
                        $"Policy is provided by {d.Company.CompanyName()} Insurance"
                    };
                    return string.Join(" ", sentences.OrderBy(x => d.Random.Int()));
                });
            var documents = faker.Generate(documentCount);
            return documents;
        }
        public async Task<float[]> GetEmbeddings(string text)
        {
            var response = await embeddingClient.GenerateEmbeddingAsync(text);
            return response.Value.ToFloats().ToArray();
        }
        public async Task UplaodDocuments(List<AzureSearchDocument> documents)
        {
            foreach (var doc in documents)
            {
                doc.ContentVector = await GetEmbeddings(doc.Content);
                var blobClient = blobContainerClient.GetBlobClient($"{doc.Id}.json");
                var jsonContent = System.Text.Json.JsonSerializer.Serialize(doc, new JsonSerializerOptions() { WriteIndented = true });
                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
                await blobClient.UploadAsync(stream, overwrite: true);
            }
        }
        public async Task UplaodVideo(string videoId, Stream stream)
        {
            var blobClient = blobContainerClient.GetBlobClient($"{videoId}.mp4");
            BlobUploadOptions blobUploadOptions = new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders()
                {
                    ContentType = "video/mp4"
                },
                TransferOptions = new Azure.Storage.StorageTransferOptions()
                {
                    InitialTransferSize = 4 * 1024 * 1024,
                    MaximumTransferSize = 4 * 1024 * 1024,
                    MaximumConcurrency = 4
                }
            };
            await blobClient.UploadAsync(stream, blobUploadOptions);
        }
        public async Task CreateOrUpdateIndex()
        {
            var searchIndex = new SearchIndex("insurance-documents-index")
            {
                Fields = new FieldBuilder().Build(typeof(AzureSearchDocument)),
                VectorSearch = new VectorSearch
                {
                    Profiles = { new VectorSearchProfile("vector-profile", "vector-algorithm") },
                    Algorithms = { new HnswAlgorithmConfiguration("vector-algorithm") }
                }
            };

            AzureKeyCredential azureKeyCredential = new AzureKeyCredential(searchKey);
            SearchIndexClient searchIndexClient = new SearchIndexClient(new Uri(searchEndPoint), azureKeyCredential);
            await searchIndexClient.CreateOrUpdateIndexAsync(searchIndex);
        }
        public async Task CreateOrUpdateIndexer()
        {
            var indexerClient = new SearchIndexerClient(new Uri(searchEndPoint), new AzureKeyCredential(searchKey));
            var indexer = new SearchIndexer(name: "insurance-documents-indexer", "azure-ai-search-demo", "insurance-documents-index")
            {
                Parameters = new IndexingParameters
                {
                    IndexingParametersConfiguration = new IndexingParametersConfiguration
                    {
                        ["parsingMode"] = "json"
                    }
                },
                FieldMappings =
                {
                    new FieldMapping("Id"){TargetFieldName="Id"},
                    new FieldMapping("Content"){TargetFieldName="Content"},
                    new FieldMapping("Insurer"){TargetFieldName="Insurer"},
                    new FieldMapping("Title"){TargetFieldName="Title"},
                    new FieldMapping("Tags"){TargetFieldName="Tags"},
                    new FieldMapping("PremiumAmount"){TargetFieldName="PremiumAmount"},
                    new FieldMapping("IsActive"){TargetFieldName="IsActive"},
                    new FieldMapping("ContentVector"){TargetFieldName="ContentVector"}
                }
            };
            await indexerClient.CreateOrUpdateIndexerAsync(indexer);
        }
        public async Task<Response<SearchResults<SearchDocument>>> SearchDocuments(string queryText, SearchOptions searchOptions)
        {
            var searchResult = await searchClient.SearchAsync<SearchDocument>(queryText, searchOptions);
            return searchResult;
        }
    }
}
