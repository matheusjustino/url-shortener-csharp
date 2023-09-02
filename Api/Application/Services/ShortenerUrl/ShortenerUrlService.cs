namespace url_shortener.Api.Application.Services;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using url_shortener.Api.Domain.Entities;
using url_shortener.Api.Infrastructure.Persistence;
using JsonSerializer = System.Text.Json.JsonSerializer;

public class ShortenerUrlService : IShortenerUrlService
{
    private readonly ILogger<ShortenerUrlService> _logger;

    private readonly AppDbContext _context;

    private readonly ICachingService _cachingService;

    public const int NumberOfCharsInShortLink = 7;

    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private readonly Random _random = new();

    public ShortenerUrlService(ILogger<ShortenerUrlService> logger, AppDbContext context, ICachingService cachingService)
    {
        this._logger = logger;
        this._context = context;
        this._cachingService = cachingService;
    }

    public async Task<string> GenerateUniqueCode(string LongUrl, string ShortUrl)
    {
        this._logger.LogInformation($"GenerateUniqueCode - LongUrl: {LongUrl} - ShortUrl: {ShortUrl}");

        if (!Uri.TryCreate(LongUrl, UriKind.Absolute, out _))
        {
            throw new BadHttpRequestException("The specified URL is invalid");
        }

        var code = "";

        while (true)
        {
            var codeChars = new char[NumberOfCharsInShortLink];

            for (var i = 0; i < NumberOfCharsInShortLink; i++)
            {
                var randomIndex = this._random.Next(Alphabet.Length - 1);
                codeChars[i] = Alphabet[randomIndex];
            }

            code = new string(codeChars);

            if (!await this._context.ShortenedUrl.AnyAsync(s => s.Code == code))
            {
                break;
            }
        }

        var shortenedUrl = new ShortenedUrl
        {
            LongUrl = LongUrl,
            Code = code,
            ShortUrl = ShortUrl + $"/{code}",
        };

        await this._context.ShortenedUrl.AddAsync(shortenedUrl);
        await this._context.SaveChangesAsync();
        await this._cachingService.SetAsync(code, shortenedUrl.ToString());

        return shortenedUrl.ShortUrl;
    }

    public async Task<string> ResolveCodeUrl(string CodeUrl)
    {
        this._logger.LogInformation($"Resolve Code Url - CodeUrl: {CodeUrl}");

        var cacheCode = await this._cachingService.GetAsync(CodeUrl);
        if (!string.IsNullOrWhiteSpace(cacheCode))
        {
            var cachedShortenedUrl = JsonConvert.DeserializeObject<ShortenedUrl>(cacheCode);
            return cachedShortenedUrl.LongUrl;
        }

        var shortenedUrl = await this._context.ShortenedUrl.FirstOrDefaultAsync(s => s.Code == CodeUrl);
        if (shortenedUrl is null)
        {
            throw new BadHttpRequestException("URL not found");
        }

        await this._cachingService.SetAsync(CodeUrl, shortenedUrl.ToString());

        return shortenedUrl.LongUrl;
    }
}