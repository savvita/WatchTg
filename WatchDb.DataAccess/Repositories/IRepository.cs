namespace WatchDb.DataAccess.Repositories
{
    public interface IRepository<T> where T: class
    {
        Task<T> CreateAsync(T item);
        Task<IEnumerable<T>> GetAsync();
        Task<T?> GetAsync(int id);
        Task<T?> UpdateAsync(T item);
        Task<bool> DeleteAsync(int id);
    }
}
