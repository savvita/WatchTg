using Newtonsoft.Json;
using StackExchange.Redis;

namespace WatchDb.API.Cache
{
    public class CacheService : ICacheService
    {
        IDatabase db;

        public CacheService()
        {
            db = ConnectionService.Connection.GetDatabase();
        }

        public async Task<T?> GetData<T>(string key)
        {
            var value = await db.StringGetAsync(key);

            if(!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value!);
            }

            return default;
        }

        public async Task<bool> RemoveData(string key)
        {
            return await db.KeyDeleteAsync(key);
        }

        public async Task<bool> SetData<T>(string key, T value, DateTimeOffset offset)
        {
            return await db.StringSetAsync(key, JsonConvert.SerializeObject(value), offset.DateTime.Subtract(DateTime.Now));
        }
    }
}
