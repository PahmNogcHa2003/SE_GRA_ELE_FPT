using Application.Interfaces.Cache;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        /// <summary>
        /// Lấy dữ liệu từ cache
        /// </summary>
        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;

            return JsonSerializer.Deserialize<T>(value!);
        }

        /// <summary>
        /// Lưu dữ liệu vào cache với thời gian hết hạn tùy chọn
        /// </summary>
        public async Task SetAsync<T>(string key, T data, TimeSpan? expiry = null)
        {
            if (data == null) return;

            var json = JsonSerializer.Serialize(data);

            // StackExchange.Redis nhận TimeSpan? trực tiếp (phiên bản mới)
            await _db.StringSetAsync(key, json, (Expiration)expiry);
        }

        /// <summary>
        /// Xóa dữ liệu trong cache
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
