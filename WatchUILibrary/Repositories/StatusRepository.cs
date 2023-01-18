using WatchDb.DataAccess.Models;
using WatchDb.DataAccess.Repositories;
using WatchUILibrary.Models;

namespace WatchUILibrary.Repositories
{
    public class StatusRepository
    {
        private DbContext context;
        public StatusRepository(DbContext context)
        {
            this.context = context;
        }
        public async Task<List<Status>> GetAsync()
        {
            return (await context.Statuses.GetAsync()).Select(model => new Status(model)).ToList();
        }

        public async Task<Status?> GetAsync(int id)
        {
            var model = await context.Statuses.GetAsync(id);

            return model != null ? new Status(model) : null;
        }

        public async Task<Status?> UpdateAsync(Status status)
        {
            var model = await context.Statuses.UpdateAsync(new StatusModel()
            {
                Id = status.Id,
                StatusName = status.StatusName
            });

            if (model == null)
            {
                return null;
            }

            return new Status(model);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await context.Statuses.DeleteAsync(id);
        }

        public async Task<Status> CreateAsync(Status status)
        {
            return new Status(await context.Statuses.CreateAsync(new StatusModel()
            {
                StatusName = status.StatusName,
            }));
        }
    }
}
