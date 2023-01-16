namespace WatchDb.DataAccess.Repositories
{
    public interface IReadRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
    }
}
