namespace url_shortener.Api.Application.Services;

public interface ICachingService
{
    Task SetAsync(string Key, string Value);

    Task<string?> GetAsync(string Key);
}