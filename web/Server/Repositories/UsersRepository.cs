using Dapper;
using Shearlegs.Web.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Shearlegs.Web.Server.Repositories
{
    public class UsersRepository
    {
        private readonly SqlConnection connection;

        public UsersRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<UserModel> AddUserAsync(UserModel user)
        {
            const string sql = "INSERT INTO dbo.Users (Name, Role, PasswordHash) " +
                "OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.Role, INSERTED.CreateDate " + 
                "VALUES (@Name, @Role, HASHBYTES('SHA2_512', @Password));";

            return await connection.QuerySingleOrDefaultAsync<UserModel>(sql, user);
        }

        public async Task UpdateUserAsync(UserModel user)
        {
            const string sql = "UPDATE dbo.Users SET Role = @Role WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, user);
        }

        public async Task<UserModel> GetUserAsync(int id)
        {
            const string sql = "SELECT Id, Name, Role, CreateDate FROM dbo.Users WHERE Id = @id";
            return await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { id });
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync()
        {
            const string sql = "SELECT Id, Name, Role, CreateDate FROM dbo.Users;";
            return await connection.QueryAsync<UserModel>(sql);
        }

        public async Task<UserModel> GetUserAsync(string name, string password)
        {
            const string sql = "SELECT Id, Name, Role, CreateDate FROM dbo.Users " +
                "WHERE Name = @name AND PasswordHash = HASHBYTES('SHA2_512', @password);";

            return await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { name, password });
        }
    }
}
