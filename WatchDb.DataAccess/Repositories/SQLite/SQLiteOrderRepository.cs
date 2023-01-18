using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLiteOrderRepository : IOrderRepository
    {
        private DBConfig configuration;

        public SQLiteOrderRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<OrderModel> CreateAsync(OrderModel order)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into Orders(Date, UserId, StatusId) values(@Date, @UserId, @StatusId); select Id from Orders order by Id desc limit 1;", order);
            order.Id = id;
            return order;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.ExecuteAsync("delete from Orders where Id = @Id", new { Id = id }) != 0;
        }

        public async Task<IEnumerable<OrderModel>> GetAsync()
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryAsync<OrderModel, StatusModel, OrderModel>("select * from Orders left join Statuses on Orders.StatusId = Statuses.Id",
                (o, s) => new OrderModel()
                {
                    Id = o.Id,
                    Date = o.Date,
                    StatusId = o.StatusId,
                    UserId = o.UserId,
                    Status = s
                });
        }

        public async Task<IEnumerable<OrderModel>> GetAsync(int? userId, int? statusId)
        {
            string sql = @"select * from Orders left join Statuses on Orders.StatusId = Statuses.Id";

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

            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);

            return await connection.QueryAsync<OrderModel, StatusModel, OrderModel>(sql, (o, s) => new OrderModel()
            {
                Id = o.Id,
                Date = o.Date,
                StatusId = o.StatusId,
                UserId = o.UserId,
                Status = s
            }, parameters);
        }

        public async Task<OrderModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            var model = await connection.QueryFirstOrDefaultAsync<OrderModel>("select * from Orders where Id = @Id", new { Id = id });

            if(model != null)
            {
                model.Status = await connection.QueryFirstOrDefaultAsync<StatusModel>("select * from Statuses where Id = @Id", 
                    new { Id = model.StatusId });
            }

            return model;
        }

        public async Task<OrderModel?> UpdateAsync(OrderModel order)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Orders set Date = @Date, StatusId = @StatusId, UserId = @UserId where Id = @Id", order);

            return rows != 0 ? order : null;
        }
    }
}
