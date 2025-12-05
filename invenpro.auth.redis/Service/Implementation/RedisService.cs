using invenpro.auth.common.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace invenpro.auth.redis.Service.Implementation;

internal class RedisService : IRedisService
{
    private readonly IDatabase _database;

    public RedisService(IOptions<RedisSettings> _settigs)
    {
        var config = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            EndPoints = { _settigs.Value.Endpoint ?? string.Empty },
            Ssl = !_settigs.Value.Local,
            AllowAdmin = true,
        };

        var lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(config));
        ConnectionMultiplexer connection = lazyConnection.Value;
        _database = connection.GetDatabase();
    }

    public async Task<string?> GetInformationAsync(string key)
        => await _database.StringGetAsync(key);

    public async Task<bool> SaveInformationAsync(string key, string value, TimeSpan expiration)
        => await _database.StringSetAsync(key, value, expiration);

    public async Task<bool> DeleteInformationAsync(string key)
        => await _database.KeyDeleteAsync(key);

    public async Task<TimeSpan?> GetExpireTime(string key)
            => await _database.KeyTimeToLiveAsync(key);
}