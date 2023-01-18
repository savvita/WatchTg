namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLDbContext : DbContext
    {
        public SQLDbContext(DBConfig configuration) : base(
            new SQLCategoryRepository(configuration),
            new SQLOrderDetailRepository(configuration),
            new SQLOrderRepository(configuration),
            new SQLProducerRepository(configuration),
            new SQLStatusRepository(configuration),
            new SQLUserRepository(configuration),
            new SQLWatchRepository(configuration))
        {

        }
    }
}
