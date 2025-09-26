using Azure;
using Azure.Data.Tables;

namespace ClassLibrary.Entities
{
    public class CustomerHistoryEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string History { get; set; }
    }
}
