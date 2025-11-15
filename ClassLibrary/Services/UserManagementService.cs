using Azure.Data.Tables;
using ClassLibrary.Entities;
using System.Text;

namespace ClassLibrary.Services
{
    public class UserManagementService(TableClient tableClient)
    {
        public async Task<string> CreateNewUserAccount(
            string fullName,
            string emailAddress,
            string phoneNumber,
            string password)
        {
            string userId = Guid.NewGuid().ToString();
            var newUserAccount = new UserAccountEntity()
            {
                Id = userId,
                FullName = fullName,
                EmailAddress = emailAddress,
                PhoneNumber = phoneNumber,
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password)),
                PartitionKey = "UserAccount",
                RowKey = userId
            };
            await tableClient.AddEntityAsync(newUserAccount);
            return userId;
        }
        public async Task<List<UserAccountEntity>> GetAllUsers()
        {
            var users = new List<UserAccountEntity>();
            await foreach (var user in tableClient.QueryAsync<UserAccountEntity>())
            {
                users.Add(user);
            }
            return users;
        }
    }
}
