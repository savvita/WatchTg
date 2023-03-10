using Dapper;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLOrderDetailRepository : IOrderDetailRepository
    {
        private DBConfig configuration;

        public SQLOrderDetailRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<OrderDetailModel> CreateAsync(OrderDetailModel model)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into OrderDetails values(@OrderId, @WatchId, @Count, @UnitPrice); select SCOPE_IDENTITY();", model);
            model.Id = id;
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.ExecuteAsync("delete OrderDetails where Id = @Id", new { Id = id }) != 0;
        }

        public async Task<IEnumerable<OrderDetailModel>> GetAsync()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<OrderDetailModel>("select * from OrderDetails");
        }

        public async Task<OrderDetailModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<OrderDetailModel>("select * from OrderDetails where Id = @Id", new { Id = id });
        }
        public async Task<IEnumerable<OrderDetailModel>> GetByOrderId(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<OrderDetailModel>("select * from OrderDetails where OrderId = @Id", new { Id = id });
        }

        public async Task<OrderDetailModel?> UpdateAsync(OrderDetailModel model)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update OrderDetails set OrderId = @OrderId, WatchId = @WatchId, Count = @Count, UnitPrice = @UnitPrice where Id = @Id", model);

            return rows != 0 ? model : null;
        }
    }
}
