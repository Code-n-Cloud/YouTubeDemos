using Azure.Data.Tables;
using ClassLibrary.Entities;
using System.Text.Json.Nodes;

namespace ClassLibrary.Services
{
    public class StorageAccountService(TableClient tableClient)
    {
        public async Task<JsonObject> GetCustomerHistoryEntity(string partitionKey, string rowKey)
        {
            try
            {
                var response = await tableClient.GetEntityAsync<CustomerHistoryEntity>(partitionKey, rowKey);
                return JsonObject.Parse(response.Value.History).AsObject();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
