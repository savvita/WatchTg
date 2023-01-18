using WatchDb.DataAccess.Models;
using WatchDb.DataAccess.Repositories;
using WatchUILibrary.Models;

namespace WatchUILibrary.Repositories
{
    public class CategoryRepository
    {
        private DbContext context;
        public CategoryRepository(DbContext context)
        {
            this.context = context;
        }
        public async Task<List<Category>> GetAsync()
        {
            return (await context.Categories.GetAsync()).Select(model => new Category(model)).ToList();
        }

        public async Task<Category?> GetAsync(int id)
        {
            var model = await context.Categories.GetAsync(id);

            return model != null ? new Category(model) : null;
        }

        public async Task<Category?> UpdateAsync(Category category)
        {
            var model = await context.Categories.UpdateAsync(new CategoryModel()
            {
                Id = category.Id,
                CategoryName = category.CategoryName
            });

            if (model == null)
            {
                return null;
            }

            return new Category(model);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await context.Categories.DeleteAsync(id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            return new Category(await context.Categories.CreateAsync(new CategoryModel()
            {
                CategoryName = category.CategoryName,
            }));
        }
    }
}
