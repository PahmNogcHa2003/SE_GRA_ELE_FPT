using Application.Interfaces.Location;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Infrastructure.Repositories.Location
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly string _apiKey;
        private const string BaseApiUrl = "https://tinhthanhpho.com/api/v1/";

        public LocationRepository(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _apiKey = Environment.GetEnvironmentVariable("TINHTHANHPHO_API_KEY") ?? "";
        }

        /// <summary>
        /// Lấy danh sách tỉnh/thành phố (API mới)
        /// </summary>
        public async Task<IEnumerable<Province>> GetProvincesAsync()
        {
            const string cacheKey = "provinces_v1";

            if (_cache.TryGetValue(cacheKey, out List<Province> cached))
                return cached;

            var client = CreateClient();
            var response = await client.GetAsync("new-provinces?limit=100");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<ApiResponse<List<Province>>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var provinces = json?.Data ?? new List<Province>();
            _cache.Set(cacheKey, provinces, TimeSpan.FromDays(7));

            return provinces;
        }

        /// <summary>
        /// Lấy danh sách phường/xã theo mã tỉnh (API mới)
        /// </summary>
        public async Task<IEnumerable<Ward>> GetWardsByProvinceAsync(string provinceCode)
        {
            string cacheKey = $"wards_v1_{provinceCode}";

            if (_cache.TryGetValue(cacheKey, out List<Ward> cached))
                return cached;

            var client = CreateClient();
            var response = await client.GetAsync($"new-provinces/{provinceCode}/wards?limit=200");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<ApiResponse<List<Ward>>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var wards = json?.Data ?? new List<Ward>();
            _cache.Set(cacheKey, wards, TimeSpan.FromDays(7));

            return wards;
        }

        /// <summary>
        /// Lấy địa chỉ đầy đủ (API mới)
        /// </summary>
        public async Task<FullAddress?> GetFullAddressAsync(string provinceCode, string wardCode)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"new-full-address?provinceCode={provinceCode}&wardCode={wardCode}");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<ApiResponse<FullAddress>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return json?.Data;
        }

        // ===================== Helper Methods =====================

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(BaseApiUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(_apiKey))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            return client;
        }

        // ===================== Generic Response Wrapper =====================

        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
        }
    }
}
