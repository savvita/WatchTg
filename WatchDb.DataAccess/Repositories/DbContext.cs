namespace WatchDb.DataAccess.Repositories
{
    public class DbContext
    {
        public ICategoryRepository Categories { get; }
        public IOrderDetailRepository OrderDetails { get; }
        public IOrderRepository Orders { get; }
        public IProducerRepository Producers { get; }
        public IStatusRepository Statuses { get; }
        public IUserRepository Users { get; }
        public IWatchRepository Watches { get; }
        public DbContext(ICategoryRepository categories, IOrderDetailRepository orderDetails, IOrderRepository orders, IProducerRepository producers, IStatusRepository statuses, IUserRepository users, IWatchRepository watches)
        {
            Categories = categories;
            OrderDetails = orderDetails;
            Orders = orders;
            Producers = producers;
            Statuses = statuses;
            Users = users;
            Watches = watches;
        }
    }
}
