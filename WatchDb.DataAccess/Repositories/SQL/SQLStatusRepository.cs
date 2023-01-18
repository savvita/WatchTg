using Dapper;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLStatusRepository : IStatusRepository
    {
        private DBConfig configuration;

        public SQLStatusRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<StatusModel> CreateAsync(StatusModel item)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into Statuses values(@StatusName); select SCOPE_IDENTITY();", item);
            item.Id = id;
            return item;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.ExecuteAsync("delete Statuses where Id = @Id", new { Id = id }) != 0;
        }

        public async Task<IEnumerable<StatusModel>> GetAsync()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<StatusModel>("select * from Statuses");
        }

        public async Task<StatusModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<StatusModel>("select * from Statuses where Id = @Id", new { Id = id });
        }

        public async Task<StatusModel?> UpdateAsync(StatusModel item)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Statuses set StatusName = @StatusName where Id = @Id", item);

            return rows != 0 ? item : null;
        }
    }
}
