using Application.Interfaces;
using Application.Interfaces.Location;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Repositories.Location;

public class LocationRepository : ILocationRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private const string BaseApiUrl = "https://provinces.open-api.vn/api/v2/";

    public LocationRepository(IHttpClientFactory httpClientFactory, IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
    }

    public async Task<IEnumerable<Province>> GetProvincesAsync()
    {
        const string cacheKey = "provinces_v2";
        if (_cache.TryGetValue(cacheKey, out List<Province> provinces))
        {
            return provinces ?? new List<Province>();
        }

        var client = _httpClientFactory.CreateClient("ProvincesAPI");
        var response = await client.GetAsync("p/");
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<Province>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result is not null)
        {
            _cache.Set(cacheKey, result, TimeSpan.FromDays(7));
        }

        return result ?? new List<Province>();
    }

    public async Task<IEnumerable<Ward>> GetWardsByProvinceAsync(int provinceCode)
    {
        string cacheKey = $"wards_of_province_v2_{provinceCode}";
        if (_cache.TryGetValue(cacheKey, out List<Ward> wards))
        {
            return wards ?? new List<Ward>();
        }

        var client = _httpClientFactory.CreateClient("ProvincesAPI");
        var response = await client.GetAsync($"w/?province_code={provinceCode}");
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<List<Ward>>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result is not null)
        {
            _cache.Set(cacheKey, result, TimeSpan.FromDays(7));
        }

        return result ?? new List<Ward>();
    }
}