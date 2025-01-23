using AppGlobal.Config;
using Microsoft.Extensions.Options;
using OnaxTools.Services.StackExchangeRedis.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace AppGlobal.Services;


public class CacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly string _appKey;
    private readonly AppSettings _appSettings;
    public CacheService(IOptions<AppSettings> appsettings, IConnectionMultiplexer cxnMultiplexer)
    {
        _appSettings = appsettings.Value; ;
        _appKey = _appSettings.AppKey;
        _db ??= cxnMultiplexer.GetDatabase();
    }
    public async Task<T?> GetData<T>(string key)
    {
        RedisValue result = await _db.StringGetAsync($"{_appKey}:{key}");
        if (result.HasValue)
        {
            return await Task.Run(() => JsonSerializer.Deserialize<T>(result.ToString()));
        }
        return default;
    }

    public async Task<bool> SetData<T>(string key, T value, int ttl)
    {
        TimeSpan expiryTime = TimeSpan.FromSeconds(ttl); // ttl.DateTime.Subtract(DateTime.Now);
        bool isSet = false;
        try
        {
            isSet = await _db.StringSetAsync($"{_appKey}:{key}", JsonSerializer.Serialize(value), expiryTime);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return isSet;
    }

    public async Task<T?> GetSessionData<T>(string key)
    {
        var result = await _db.StringGetAsync($"{_appKey}Session:{key}");
        if (result.HasValue)
        {
            return await Task.Run(() => JsonSerializer.Deserialize<T>(result.ToString()));
        }
        return default;
    }
    public async Task<bool> SetSessionData<T>(string key, T value, int ttl)
    {
        TimeSpan expiryTime = TimeSpan.FromSeconds(ttl);
        bool isSet = false;
        try
        {
            isSet = await _db.StringSetAsync($"{_appKey}Session:{key}", JsonSerializer.Serialize(value), expiryTime);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return isSet;
    }

    public async Task<bool> RemoveData(string key)
    {
        bool _isKeyExist = _db.KeyExists($"{_appKey}:{key}");
        if (_isKeyExist == true)
        {
            return await _db.KeyDeleteAsync($"{_appKey}:{key}");
        }
        return false;
    }
}
