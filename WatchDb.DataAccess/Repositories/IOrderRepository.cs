using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public interface IOrderRepository : IRepository<OrderModel>
    {
        Task<IEnumerable<OrderModel>> GetAsync(int? userId, int? statusId);
    }
}
