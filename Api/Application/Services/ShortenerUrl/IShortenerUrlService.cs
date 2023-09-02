namespace url_shortener.Api.Application.Services;

public interface IShortenerUrlService
{
    Task<string> GenerateUniqueCode(string LongUrl, string ShortUrl);

    Task<string> ResolveCodeUrl(string CodeUrl);
}