namespace WatchDb.DataAccess.Repositories
{
    public interface IRepository<T> where T: class
    {
        Task<T> CreateAsync(T item);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T?> UpdateAsync(T item);
        Task<bool> DeleteAsync(int id);
    }
}
