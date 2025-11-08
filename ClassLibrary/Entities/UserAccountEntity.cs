using Azure;
using Azure.Data.Tables;

namespace ClassLibrary.Entities
{
    public class UserAccountEntity : ITableEntity
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
