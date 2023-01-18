using System.Data;
using System.Data.SqlClient;
using WatchDb.DataAccess.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLiteCategoryRepository : ICategoryRepository
    {
        private DBConfig configuration;

        public SQLiteCategoryRepository(DBConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task<CategoryModel> CreateAsync(CategoryModel category)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int id = await connection.QueryFirstOrDefaultAsync<int>("insert into Categories (CategoryName) values(@CategoryName); select Id from Categories order by Id desc limit 1;", category);
            category.Id = id;
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            await connection.ExecuteAsync("update Watches set CategoryId = null where CategoryId = @Id", new { Id = id });
            return await connection.ExecuteAsync("delete from Categories where Id = @Id", new { Id = id }) != 0;
        }

        public async Task<IEnumerable<CategoryModel>> GetAsync()
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryAsync<CategoryModel>("select * from Categories");
        }

        public async Task<CategoryModel?> GetAsync(int id)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<CategoryModel>("select * from Categories where Id = @Id", new { Id = id });
        }

        public async Task<CategoryModel?> UpdateAsync(CategoryModel category)
        {
            using IDbConnection connection = new SqliteConnection(configuration.ConnectionString);
            int rows = await connection.ExecuteAsync("update Categories set CategoryName = @CategoryName where Id = @Id", category);

            return rows != 0 ? category : null;
        }
    }
}
