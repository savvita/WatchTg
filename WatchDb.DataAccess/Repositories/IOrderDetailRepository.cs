using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public interface IOrderDetailRepository : IRepository<OrderDetailModel>
    {
        Task<IEnumerable<OrderDetailModel>> GetByOrderId(int id);
    }
}
