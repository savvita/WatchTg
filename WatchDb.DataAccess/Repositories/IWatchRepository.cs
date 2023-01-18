using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public interface IWatchRepository : IRepository<WatchModel>
    {
        Task<IEnumerable<WatchModel>> GetAsync( string title,
                                                List<int>? categoryIds = null,
                                                List<int>? producerIds = null,
                                                decimal? minPrice = null,
                                                decimal? maxPrice = null,
                                                bool? onSale = null);
    }
}
