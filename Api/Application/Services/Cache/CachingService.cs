namespace url_shortener.Api.Application.Services;

using Microsoft.Extensions.Caching.Distributed;

public class CachingService : ICachingService
{
    private readonly ILogger<CachingService> _logger;

    private readonly IDistributedCache _cache;

    private readonly DistributedCacheEntryOptions _cacheOpts;

    public CachingService(ILogger<CachingService> logger, IDistributedCache cache)
    {
        this._logger = logger;
        this._cache = cache;
        this._cacheOpts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3600), // 1h
            SlidingExpiration = TimeSpan.FromSeconds(1200),
        };
    }

    public async Task SetAsync(string Key, string Value)
    {
        this._logger.LogInformation($"SetAsync - Key: {Key} - Value: {Value}");
        await this._cache.SetStringAsync(Key, Value, this._cacheOpts);
    }

    public async Task<string?> GetAsync(string Key)
    {
        this._logger.LogInformation($"GetAsync - Key: {Key}");
        return await this._cache.GetStringAsync(Key);
    }
}