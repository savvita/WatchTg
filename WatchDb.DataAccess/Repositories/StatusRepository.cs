using Dapper;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public class StatusRepository : IReadRepository<StatusModel>
    {
        private DBConfig configuration;

        public StatusRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<StatusModel>> GetAll()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<StatusModel>("select * from Statuses");
        }

        public async Task<StatusModel> GetById(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<StatusModel>("select * from Statuses where Id = @Id", new { Id = id });
        }
    }
}
