using Dapper;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public class ProducerRepository : IRepository<ProducerModel>
    {
        private DBConfig configuration;

        public ProducerRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<ProducerModel> CreateAsync(ProducerModel producer)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync("insert into Producers values(@ProducerName); select SCOPE_IDENTITY();", producer);
            producer.Id = id;
            return producer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            await connection.ExecuteAsync("update Watches set ProducerId = null where ProducerId = @Id", new { Id = id });
            return (await connection.ExecuteAsync("delete Producers where Id = @Id", new { Id = id })) != 0;
        }

        public async Task<IEnumerable<ProducerModel>> GetAll()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<ProducerModel>("select * from Producers");
        }

        public async Task<ProducerModel> GetById(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<ProducerModel>("select * from Producers where Id = @Id", new { Id = id });
        }

        public async Task<ProducerModel?> UpdateAsync(ProducerModel producer)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Producers set ProducerName = @ProducerName where Id = @Id", producer);

            return rows != 0 ? producer : null;
        }
    }
}
