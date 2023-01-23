namespace WatchDb.API.Cache
{
    public interface ICacheService
    {
        Task<T?> GetData<T>(string key);
        Task<bool> SetData<T>(string key, T value, DateTimeOffset offset);
        Task<bool> RemoveData(string key);
    }
}
