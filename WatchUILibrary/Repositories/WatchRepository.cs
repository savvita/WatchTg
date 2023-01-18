using WatchDb.DataAccess.Models;
using WatchDb.DataAccess.Repositories;
using WatchUILibrary.Models;

namespace WatchUILibrary.Repositories
{
    public class WatchRepository
    {
        private DbContext context;
        public WatchRepository(DbContext context)
        {
            this.context = context;
        }
        public async Task<List<Watch>> GetAsync()
        {
            return (await context.Watches.GetAsync()).Select(model => new Watch(model)).ToList();
        }

        public async Task<Watch?> GetAsync(int id)
        {
            var model = await context.Watches.GetAsync(id);

            return model != null ? new Watch(model) : null;
        }

        public async Task<List<Watch>> GetAsync(string title,
                                                List<int>? categoryIds = null,
                                                List<int>? producerIds = null,
                                                decimal? minPrice = null,
                                                decimal? maxPrice = null,
                                                bool? onSale = null)
        {
            return (await context.Watches.GetAsync(title, categoryIds, producerIds, minPrice, maxPrice, onSale))
                .Select(model => new Watch(model)).ToList();
        }

        public async Task<Watch?> UpdateAsync(Watch watch)
        {
            var model = await context.Watches.UpdateAsync(new WatchModel()
            {
                Id = watch.Id,
                Available = watch.Available,
                CategoryId = watch.Category != null ? watch.Category.Id : null,
                Image = watch.Image,
                OnSale = watch.OnSale,
                Price = watch.Price,
                ProducerId = watch.Producer != null ? watch.Producer.Id : null
            });

            if (model == null)
            {
                return null;
            }

            return new Watch(model);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await context.Watches.DeleteAsync(id);
        }

        public async Task<Watch> CreateAsync(Watch watch)
        {
            return new Watch(await context.Watches.CreateAsync(new WatchModel()
            {
                Title = watch.Title,
                Available = watch.Available,
                CategoryId = watch.Category != null ? watch.Category.Id : null,
                Image = watch.Image,
                OnSale = watch.OnSale,
                Price = watch.Price,
                ProducerId = watch.Producer != null ? watch.Producer.Id : null
            }));
        }
    }
}
