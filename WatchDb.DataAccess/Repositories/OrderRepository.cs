using Dapper;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public class OrderRepository : IRepository<OrderModel>
    {
        private DBConfig configuration;

        public OrderRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<OrderModel> CreateAsync(OrderModel order)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync("insert into Orders values(@Date, @UserId, @StatusId); select SCOPE_IDENTITY();", order);
            order.Id = id;
            return order;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return (await connection.ExecuteAsync("delete Orders where Id = @Id", new { Id = id })) != 0;
        }

        public async Task<IEnumerable<OrderModel>> GetAll()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<OrderModel>("select * from Orders");
        }

        public async Task<IEnumerable<OrderModel>> GetAllAsync(int? userId, int? statusId)
        {
            string sql = @"select * from Orders";

            DynamicParameters parameters = new DynamicParameters();
            List<string> filters = new List<string>();

            if (userId != null)
            {
                filters.Add("UserId = @UserId");
                parameters.Add("@UserId", userId);
            }

            if (statusId != null)
            {
                filters.Add("StatusId = @StatusId");
                parameters.Add("@StatusId", statusId);
            }

            if (filters.Count > 0)
            {
                sql += $" where {string.Join(" and ", filters)}";
            }

            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            return await connection.QueryAsync<OrderModel>(sql, parameters);
        }

        public async Task<OrderModel> GetById(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<OrderModel>("select * from Orders where Id = @Id", new { Id = id });
        }

        public async Task<OrderModel?> UpdateAsync(OrderModel order)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Users set Date = @Date, StatusId = @StatusId, UserId = @UserId where Id = @Id", order);

            return rows != 0 ? order : null;
        }
    }
}
