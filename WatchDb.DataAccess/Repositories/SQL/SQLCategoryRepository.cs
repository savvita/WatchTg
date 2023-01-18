using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;
using Dapper;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLCategoryRepository : ICategoryRepository
    {
        private DBConfig configuration;

        public SQLCategoryRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<CategoryModel> CreateAsync(CategoryModel category)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into Categories values(@CategoryName); select SCOPE_IDENTITY();", category);
            category.Id = id;
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            await connection.ExecuteAsync("update Watches set CategoryId = null where CategoryId = @Id", new { Id = id });
            return await connection.ExecuteAsync("delete Categories where Id = @Id", new { Id = id }) != 0;
        }

        public async Task<IEnumerable<CategoryModel>> GetAsync()
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryAsync<CategoryModel>("select * from Categories");
        }

        public async Task<CategoryModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<CategoryModel>("select * from Categories where Id = @Id", new { Id = id });
        }

        public async Task<CategoryModel?> UpdateAsync(CategoryModel category)
        {
            using IDbConnection connection = new SqlConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Categories set CategoryName = @CategoryName where Id = @Id", category);

            return rows != 0 ? category : null;
        }
    }
}
