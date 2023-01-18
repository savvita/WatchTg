namespace WatchDb.DataAccess.Repositories.SQL
{
    public class SQLiteDbContext : DbContext
    {
        public SQLiteDbContext(DBConfig configuration) : base(
            new SQLiteCategoryRepository(configuration),
            new SQLiteOrderDetailRepository(configuration),
            new SQLiteOrderRepository(configuration),
            new SQLiteProducerRepository(configuration),
            new SQLiteStatusRepository(configuration),
            new SQLiteUserRepository(configuration),
            new SQLiteWatchRepository(configuration))
        {

        }
    }
}
