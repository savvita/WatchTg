using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDb.DataAccess.Models;
using WatchDb.DataAccess.Repositories;
using WatchUILibrary.Models;

namespace WatchUILibrary.Repositories
{
    public class ProducerRepository
    {
        private DbContext context;
        public ProducerRepository(DbContext context)
        {
            this.context = context;
        }
        public async Task<List<Producer>> GetAsync()
        {
            return (await context.Producers.GetAsync()).Select(model => new Producer(model)).ToList();
        }

        public async Task<Producer?> GetAsync(int id)
        {
            var model = await context.Producers.GetAsync(id);

            return model != null ? new Producer(model) : null;
        }

        public async Task<Producer?> UpdateAsync(Producer producer)
        {
            var model = await context.Producers.UpdateAsync(new ProducerModel()
            {
                Id = producer.Id,
                ProducerName = producer.ProducerName
            });

            if (model == null)
            {
                return null;
            }

            return new Producer(model);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await context.Producers.DeleteAsync(id);
        }

        public async Task<Producer> CreateAsync(Producer producer)
        {
            return new Producer(await context.Producers.CreateAsync(new ProducerModel()
            {
                ProducerName = producer.ProducerName,
            }));
        }
    }
}
