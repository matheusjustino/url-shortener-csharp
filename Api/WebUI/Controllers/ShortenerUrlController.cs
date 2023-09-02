namespace url_shortener.Api.WebUI.Controllers;

using Microsoft.AspNetCore.Mvc;
using url_shortener.Api.Application.Services;
using url_shortener.Api.Domain.DTOs.ShortenerUrl;

[Route("api")]
[ApiController]
public class ShortenerUrlController : ControllerBase
{
    private readonly IShortenerUrlService _shortenerUrlService;

    public ShortenerUrlController(IShortenerUrlService shortenerUrlService)
    {
        this._shortenerUrlService = shortenerUrlService;
    }

    [HttpPost("shortener")]
    public async Task<ActionResult<string>> GenerateUniqueCode([FromBody] CreateUrl body)
    {
        var shortUrl = $"{Request.Scheme}://{Request.Host}/api";
        var url = await this._shortenerUrlService.GenerateUniqueCode(body.Url, shortUrl);

        return Ok(url);
    }

    [HttpGet("{codeUrl}")]
    public async Task<ActionResult<string>> ResolveCodeUrl([FromRoute] string codeUrl)
    {
        var longUrl = await this._shortenerUrlService.ResolveCodeUrl(codeUrl);
        return Redirect(longUrl);
    }
}