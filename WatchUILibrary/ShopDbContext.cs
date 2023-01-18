using WatchDb.DataAccess.Repositories;
using WatchUILibrary.Repositories;

namespace WatchUILibrary
{
    public class ShopDbContext
    {
        public CategoryRepository Categories { get; }
        public OrderRepository Orders { get; }
        public ProducerRepository Producers { get; }
        public StatusRepository Statuses { get; }
        public UserRepository Users { get; }
        public WatchRepository Watches { get; }

        public ShopDbContext(DbContext context)
        {
            Categories = new CategoryRepository(context);
            Orders = new OrderRepository(context);
            Producers = new ProducerRepository(context);
            Statuses = new StatusRepository(context);
            Users = new UserRepository(context);
            Watches = new WatchRepository(context);
        }
    }
}
