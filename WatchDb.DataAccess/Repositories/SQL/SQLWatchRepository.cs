using Dapper;
using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLWatchRepository : IWatchRepository
    {
        private DBConfig configuration;

        public SQLWatchRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<WatchModel> CreateAsync(WatchModel watch)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into Watches values(@CategoryId, @ProducerId, @Title, @Price, @Available, @OnSale, @Image); select SCOPE_IDENTITY();", watch);
            watch.Id = id;
            return watch;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.ExecuteAsync("delete Watches where Id = @Id", new { Id = id }) != 0;
        }

        private WatchModel GetModel(WatchModel watch, CategoryModel category, ProducerModel producer)
        {
            WatchModel gadget = new WatchModel()
            {
                Id = watch.Id,
                Title = watch.Title,
                Price = watch.Price,
                CategoryId = watch.CategoryId,
                ProducerId = watch.ProducerId,
                Available = watch.Available,
                OnSale = watch.OnSale,
                Image = watch.Image
            };

            if (category != null && category.CategoryName != null)
            {
                gadget.Category = new CategoryModel()
                {
                    Id = category.Id,
                    CategoryName = category.CategoryName
                };
            }

            if (producer != null && producer.ProducerName != null)
            {
                gadget.Producer = new ProducerModel()
                {
                    Id = producer.Id,
                    ProducerName = producer.ProducerName
                };
            }

            return gadget;
        }

        public async Task<IEnumerable<WatchModel>> GetAsync()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            string sql = @"select * from Watches left join Categories on Watches.CategoryId = Categories.Id
                        left join Producers on Watches.ProducerId = Producers.Id";

            return await connection.QueryAsync<WatchModel, CategoryModel, ProducerModel, WatchModel>(
                sql, (w, c, p) => GetModel(w, c, p));
        }

        public async Task<IEnumerable<WatchModel>> GetAsync(string title,
                                                                List<int>? categoryIds = null,
                                                                List<int>? producerIds = null,
                                                                decimal? minPrice = null,
                                                                decimal? maxPrice = null,
                                                                bool? onSale = null)
        {
            string sql = @"select * from Watches left join Categories on Watches.CategoryId = Categories.Id
                        left join Producers on Watches.ProducerId = Producers.Id where [Title] like @Title";

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

            return await connection.QueryAsync<WatchModel, CategoryModel, ProducerModel, WatchModel>(sql, 
                (w, c, p) => GetModel(w, c, p), parameters);
        }

        public async Task<WatchModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            var model = await connection.QueryFirstOrDefaultAsync<WatchModel>("select * from Watches where Id = @Id", new { Id = id });

            if(model.CategoryId != null)
            {
                model.Category = await connection.QueryFirstOrDefaultAsync<CategoryModel>("select * from Categories where Id = @Id",
                    new { Id = model.CategoryId });
            }

            if (model.ProducerId != null)
            {
                model.Producer = await connection.QueryFirstOrDefaultAsync<ProducerModel>("select * from Producers where Id = @Id",
                    new { Id = model.ProducerId });
            }

            return model;
        }

        public async Task<WatchModel?> UpdateAsync(WatchModel watch)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Watches set Title = @Title, CategoryId = @CategoryId, ProducerId = @ProducerId, Price = @Price, Available = @Available, OnSale = @OnSale, Image = @Image where Id = @Id", watch);

            return rows != 0 ? watch : null;
        }
    }
}
