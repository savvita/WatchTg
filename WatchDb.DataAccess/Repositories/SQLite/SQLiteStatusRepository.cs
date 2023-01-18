using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLiteStatusRepository : IStatusRepository
    {
        private DBConfig configuration;

        public SQLiteStatusRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<StatusModel> CreateAsync(StatusModel item)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into Statuses(StatusName) values(@StatusName); select Id from Statuses order by Id desc limit 1;", item);
            item.Id = id;
            return item;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.ExecuteAsync("delete from Statuses where Id = @Id", new { Id = id }) != 0;
        }

        public async Task<IEnumerable<StatusModel>> GetAsync()
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryAsync<StatusModel>("select * from Statuses");
        }

        public async Task<StatusModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<StatusModel>("select * from Statuses where Id = @Id", new { Id = id });
        }

        public async Task<StatusModel?> UpdateAsync(StatusModel item)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Statuses set StatusName = @StatusName where Id = @Id", item);

            return rows != 0 ? item : null;
        }
    }
}
