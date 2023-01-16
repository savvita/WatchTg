using Dapper;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public class UserRepository : IRepository<UserModel>
    {
        private DBConfig configuration;

        public UserRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<UserModel> CreateAsync(UserModel user)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync("insert into Users values(@ChatId); select SCOPE_IDENTITY();", user);
            user.Id = id;
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return (await connection.ExecuteAsync("delete Users where Id = @Id", new { Id = id })) != 0;
        }

        public async Task<IEnumerable<UserModel>> GetAll()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<UserModel>("select * from Users");
        }

        public async Task<UserModel> GetById(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<UserModel>("select * from Users where Id = @Id", new { Id = id });
        }

        public async Task<UserModel?> UpdateAsync(UserModel user)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Users set ChatId = @ChatId where Id = @Id", user);

            return rows != 0 ? user : null;
        }
    }
}
