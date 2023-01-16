using Dapper;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public class WatchRepository : IRepository<WatchModel>
    {
        private DBConfig configuration;

        public WatchRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<WatchModel> CreateAsync(WatchModel watch)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync("insert into Watches values(@Title, @CategoryId, @ProducerId, @Price, @Available, @OnSale); select SCOPE_IDENTITY();", watch);
            watch.Id = id;
            return watch;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return (await connection.ExecuteAsync("delete Watches where Id = @Id", new { Id = id })) != 0;
        }

        public async Task<IEnumerable<WatchModel>> GetAll()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<WatchModel>("select * from Watches");
        }

        public async Task<IEnumerable<WatchModel>> GetAllAsync( string title,
                                                                List<int>? categoryIds = null, 
                                                                List<int>? producerIds = null, 
                                                                decimal? minPrice = null, 
                                                                decimal? maxPrice = null,
                                                                bool? onSale = null)
        {
            string sql = @"select * from Watches where [Title] like @Title";

            DynamicParameters parameters = new DynamicParameters();
            List<string> filters = new List<string>();

            parameters.Add("@Title", $"%{title}%");

            if (categoryIds != null)
            {
                filters.Add("CategoryId in @CategoryIds");
                parameters.Add("@CategoryIds", categoryIds);
            }

            if (producerIds != null)
            {
                filters.Add("ProducerId in @ProducerIds");
                parameters.Add("@ProducerIds", producerIds);
            }

            if (minPrice != null)
            {
                filters.Add("Price >= @MinPrice");
                parameters.Add("@MinPrice", minPrice);
            }

            if (maxPrice != null)
            {
                filters.Add("Price <= @MaxPrice");
                parameters.Add("@MaxPrice", maxPrice);
            }

            if (onSale != null)
            {
                filters.Add("OnSale = @OnSale");
                parameters.Add("@OnSale", onSale == true ? 1 : 0);
            }

            if (filters.Count > 0)
            {
                sql += " and ";
                sql += string.Join(" and ", filters);
            }

            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);

            return await connection.QueryAsync<WatchModel>(sql, parameters);
        }

        public async Task<WatchModel> GetById(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<WatchModel>("select * from Watches where Id = @Id", new { Id = id });
        }

        public async Task<WatchModel?> UpdateAsync(WatchModel watch)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Watches set Title = @Title, CategoryId = @CategoryId, ProducerId = @ProducerId, Price = @Price, Avalbale = @Available, OnSale = @OnSale where Id = @Id", watch);

            return rows != 0 ? watch : null;
        }
    }
}
