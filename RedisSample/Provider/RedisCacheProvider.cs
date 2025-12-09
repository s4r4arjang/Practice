
using StackExchange.Redis;
using System.Text.Json;

namespace RedisSample.Provider
{
    
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private readonly IDatabase _db;

        public RedisCacheProvider(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }
        public T Get<T>(string key)
        {
            var value = _db.StringGet(key);

            if (value.IsNullOrEmpty)
                return default!;

            return JsonSerializer.Deserialize<T>(value!)!;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default!;

            return JsonSerializer.Deserialize<T>(value!)!;
        }

        public void Remove(string key)
        {
            _db.KeyDelete(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
        {
            string json = JsonSerializer.Serialize(value);

            Expiration expiry = absoluteExpiration.HasValue
                ? new Expiration(absoluteExpiration.Value)
                : default; 

            _db.StringSet(key, json, expiry);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null)
        {
            string json = JsonSerializer.Serialize(value);

            Expiration expiry = absoluteExpiration.HasValue
                ? new Expiration(absoluteExpiration.Value)
                : default;

            await _db.StringSetAsync(key, json, expiry);
        }
    }
}
