namespace invenpro.auth.redis.Service;

public interface IRedisService
{
    public Task<string?> GetInformationAsync(string key);
    public Task<bool> SaveInformationAsync(string key, string value, TimeSpan expiration);
    Task<bool> DeleteInformationAsync(string key);
    Task<TimeSpan?> GetExpireTime(string key);
}