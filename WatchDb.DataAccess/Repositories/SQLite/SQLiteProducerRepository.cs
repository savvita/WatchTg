using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLiteProducerRepository : IProducerRepository
    {
        private readonly DBConfig configuration;

        public SQLiteProducerRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<ProducerModel> CreateAsync(ProducerModel producer)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into Producers(ProducerName) values(@ProducerName); select Id from Producers order by Id desc limit 1;", producer);
            producer.Id = id;
            return producer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            await connection.ExecuteAsync("update Watches set ProducerId = null where ProducerId = @Id", new { Id = id });
            return await connection.ExecuteAsync("delete from Producers where Id = @Id", new { Id = id }) != 0;
        }

        public async Task<IEnumerable<ProducerModel>> GetAsync()
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryAsync<ProducerModel>("select * from Producers");
        }

        public async Task<ProducerModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<ProducerModel>("select * from Producers where Id = @Id", new { Id = id });
        }

        public async Task<ProducerModel?> UpdateAsync(ProducerModel producer)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Producers set ProducerName = @ProducerName where Id = @Id", producer);

            return rows != 0 ? producer : null;
        }
    }
}
