using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace ClassLibrary.Entities
{
    public class AzureSearchDocument
    {
        [SimpleField(IsKey = true)]
        public string Id { get; set; }
        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string Content { get; set; }
        [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnLucene)]
        public string Insurer { get; set; }
        [SimpleField]
        public string Title { get; set; }
        [SimpleField(IsFilterable = true, IsFacetable = true)]
        public string[] Tags { get; set; }
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public double PremiumAmount { get; set; }
        [SimpleField(IsFilterable = true)]
        public bool IsActive { get; set; }
        [SimpleField(IsFilterable = false, IsSortable = false, IsFacetable = false)]
        [VectorSearchField(VectorSearchDimensions = 1536, VectorSearchProfileName = "vector-profile")]
        public float[] ContentVector { get; set; }
        [SimpleField]
        public string SpeechText { get; set; }
    }
}
