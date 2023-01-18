using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLiteUserRepository : IUserRepository
    {
        private readonly DBConfig configuration;

        public SQLiteUserRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<UserModel> CreateAsync(UserModel user)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into Users(ChatId) values(@ChatId); select Id from Users order by Id desc limit 1;", user);
            user.Id = id;
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.ExecuteAsync("delete from Users where Id = @Id", new { Id = id }) != 0;
        }

        public async Task<IEnumerable<UserModel>> GetAsync()
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryAsync<UserModel>("select * from Users");
        }

        public async Task<UserModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<UserModel>("select * from Users where Id = @Id", new { Id = id });
        }

        public async Task<UserModel> GetAsync(long chatId)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<UserModel>("select * from Users where ChatId = @Id", new { Id = chatId });
        }

        public async Task<UserModel?> UpdateAsync(UserModel user)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Users set ChatId = @ChatId where Id = @Id", user);

            return rows != 0 ? user : null;
        }
    }
}
